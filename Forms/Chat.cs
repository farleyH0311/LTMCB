using WordUp.Controls;
using WordUp.Models;
using WordUp.Services;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordUp.Controllers;

namespace WordUp.Forms
{
    public partial class Chat : BaseForm
    {
        private FirebaseHelper firebaseHelper;
        private string currentUserId;
        private User currentUser;
        private string chattingWithUserId;
        private User chattingWithUser;
        private string roomId;
        private IDisposable messageListener;
        private HashSet<string> displayedMessageKeys = new HashSet<string>();
        private bool hasPreSelectedUser = false;
        private Form1 mainform;
        public Chat(Form1 MainForm, User currentUser, User viewedUser = null)
        {
            InitializeComponent();
            this.currentUser = currentUser;
            this.mainform = MainForm;
            currentUserId = currentUser.Username;
            firebaseHelper = new FirebaseHelper("https://ltm-wu-default-rtdb.firebaseio.com/");

            if (viewedUser != null)
            {
                chattingWithUser = viewedUser;
                chattingWithUserId = viewedUser.Username;
                hasPreSelectedUser = true;
            }

            this.Load += Chat_Load;
            this.FormClosing += Chat_FormClosing;
            txtSearchUser.KeyDown += txtSearchUser_KeyDown;

        }

        private async void Chat_Load(object sender, EventArgs e)
        {
            await LoadRecentChats();

            if (hasPreSelectedUser)
            {
                OpenChatWithUser(chattingWithUser);
            }
            else
            {
                if (flowChatList.Controls.Count > 0)
                {
                    var firstChatItem = flowChatList.Controls[0] as ChatItemControl;
                    if (firstChatItem != null)
                    {
                        var user = await firebaseHelper.Client
                            .Child("Users")
                            .Child(firstChatItem.Username)
                            .OnceSingleAsync<User>();

                        if (user != null)
                            OpenChatWithUser(user);
                    }
                }
                else
                {
                    ClearChatPanel();
                }
            }
        }

        public async Task LoadRecentChats()
        {
            flowChatList.Controls.Clear();

            var recentChatsRef = firebaseHelper.Client
                .Child("recentChats")
                .Child(currentUserId);

            var recentChatsSnapshot = await recentChatsRef.OnceAsync<RecentChatInfo>();

            var sortedRecentChats = recentChatsSnapshot
                .OrderBy(chat => chat.Object.timestamp)
                .ToList();

            foreach (var chat in sortedRecentChats)
            {
                string otherUsername = chat.Key;
                var user = await firebaseHelper.Client
                    .Child("Users")
                    .Child(otherUsername)
                    .OnceSingleAsync<User>();

                if (user == null)
                    continue;

                var chatInfo = chat.Object;

                var chatItem = new ChatItemControl(user.Username, user.AvatarPath, user.Username);
                chatItem.Dock = DockStyle.Top;
                chatItem.OnChatSelected += ChatItem_OnChatSelected;

                if (chatInfo.isRead == false)
                {
                    chatItem.Font = new Font(chatItem.Font, FontStyle.Bold);
                    chatItem.BackColor = Color.FromArgb(100, 100, 100);
                }

                flowChatList.Controls.Add(chatItem);
                flowChatList.Controls.SetChildIndex(chatItem, 0);
            }
        }


        private async void ChatItem_OnChatSelected(object sender, string username)
        {
            var user = await firebaseHelper.Client
                .Child("Users")
                .Child(username)
                .OnceSingleAsync<User>();

            if (user != null)
                OpenChatWithUser(user);
        }

        private async void OpenChatWithUser(User user)
        {
            if (user == null) return;

            chattingWithUser = user;
            chattingWithUserId = user.Username;
            roomId = GetRoomId(currentUserId, chattingWithUserId);
            lblUserName.Text = chattingWithUser.Username;
            await firebaseHelper.Client
                .Child("recentChats")
                .Child(currentUserId)
                .Child(chattingWithUserId)
                .Child("isRead")
                .PutAsync(true);
            flowMessages.Controls.Clear();
            displayedMessageKeys.Clear();

            if (!string.IsNullOrEmpty(user.AvatarPath))
            {
                try
                {
                    picAvatar.ImageLocation = user.AvatarPath;
                    picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch { }
            }
            else
            {
                picAvatar.Image = null;
            }

            await LoadExistingMessages();
            StartListeningMessages();
        }

        private string GetRoomId(string u1, string u2)
        {
            return string.Compare(u1, u2) < 0 ? $"{u1}_{u2}" : $"{u2}_{u1}";
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            btnSend.Enabled = false;

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var message = new WordUp.Models.Message
            {
                SenderId = currentUserId,
                Text = text,
                Timestamp = timestamp
            };
            bool isRead = false;
            try
            {
                await firebaseHelper.Client
                    .Child("chatRooms")
                    .Child(roomId)
                    .Child("messages")
                    .PostAsync(message);

                await firebaseHelper.Client
                    .Child("recentChats")
                    .Child(currentUserId)
                    .Child(chattingWithUserId)
                    .PutAsync(new RecentChatInfo
                    {
                        timestamp = timestamp,
                        isRead = true
                    });



                await firebaseHelper.Client
                 .Child("recentChats")
                 .Child(chattingWithUserId)
                 .Child(currentUserId)
                 .PutAsync(new RecentChatInfo
                 {
                     timestamp = timestamp,
                     isRead = false
                 });



                txtMessage.Clear();
            }
            finally
            {
                btnSend.Enabled = true;
            }
        }

        private async Task LoadExistingMessages()
        {
            var messages = await firebaseHelper.Client
                .Child("chatRooms")
                .Child(roomId)
                .Child("messages")
                .OrderBy("Timestamp")
                .OnceAsync<WordUp.Models.Message>();

            foreach (var item in messages)
            {
                var msg = item.Object;
                if (msg == null || displayedMessageKeys.Contains(item.Key)) continue;

                displayedMessageKeys.Add(item.Key);
                AddMessageToFlow(msg.Text, msg.SenderId == currentUserId);
            }

            ScrollToBottom();
        }

        private void StartListeningMessages()
        {
            messageListener?.Dispose();

            messageListener = firebaseHelper.Client
                .Child("chatRooms")
                .Child(roomId)
                .Child("messages")
                .OrderBy("Timestamp")
                .AsObservable<WordUp.Models.Message>()
                .Subscribe(d =>
                {
                    if (d.Object == null || d.EventType != Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                        return;

                    if (displayedMessageKeys.Contains(d.Key))
                        return;

                    displayedMessageKeys.Add(d.Key);

                    this.Invoke((MethodInvoker)delegate
                    {
                        var msg = d.Object;
                        AddMessageToFlow(msg.Text, msg.SenderId == currentUserId);
                        ScrollToBottom();
                    });
                });
        }

        private void AddMessageToFlow(string text, bool isCurrentUser)
        {
            var bubble = new MessageBubble
            {
                MessageText = text,
                IsCurrentUser = isCurrentUser,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            flowMessages.Controls.Add(bubble);
        }

        private void ScrollToBottom()
        {
            if (flowMessages.Controls.Count > 0)
            {
                var last = flowMessages.Controls[flowMessages.Controls.Count - 1];
                flowMessages.ScrollControlIntoView(last);
            }
        }

        private void ClearChatPanel()
        {
            flowMessages.Controls.Clear();
            picAvatar.Image = null;
            lblUserName.Text = "Chưa chọn cuộc trò chuyện";
            displayedMessageKeys.Clear();
            messageListener?.Dispose();
        }

        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            messageListener?.Dispose();
            displayedMessageKeys.Clear();
        }
        private async void txtSearchUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string username = txtSearchUser.Text.Trim();

                if (string.IsNullOrEmpty(username))
                {
                    MessageBox.Show("Vui lòng nhập tên người dùng.");
                    return;
                }

                try
                {
                    var user = await firebaseHelper.Client
                        .Child("Users")
                        .Child(username)
                        .OnceSingleAsync<User>();

                    if (user != null)
                    {
                        var recentChat = await firebaseHelper.Client
                            .Child("recentChats")
                            .Child(currentUserId)
                            .Child(user.Username)
                            .OnceSingleAsync<RecentChatInfo>();

                        if (recentChat == null)
                        {
                            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            await firebaseHelper.Client
                                .Child("recentChats")
                                .Child(currentUserId)
                                .Child(user.Username)
                                .PutAsync(new RecentChatInfo
                                {
                                    timestamp = now,
                                    isRead = true
                                });

                            await firebaseHelper.Client
                                .Child("recentChats")
                                .Child(user.Username)
                                .Child(currentUserId)
                                .PutAsync(new RecentChatInfo
                                {
                                    timestamp = now,
                                    isRead = false
                                });
                        }

                        OpenChatWithUser(user);
                        await LoadRecentChats();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy người dùng.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tìm người dùng: " + ex.Message);
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtSearchUser_TextChanged(object sender, EventArgs e)
        {

        }

        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.home == null || mainform.home.IsDisposed)
                mainform.home = new Home(mainform, currentUser);
            mainform.home.Show();
            this.Close();
        }
    }
}
