using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordUp.Helpers;
using WordUp.Models;
using WordUp.Services;
using Google.Cloud.Firestore;

namespace WordUp.Forms
{
    public partial class Room : BaseForm
    {
        private readonly User currentUser;
        private Form1 mainform;
        public Room(Form1 MainForm, User user)
        {
            InitializeComponent();
            currentUser = user;
            this.mainform = MainForm;
        }

        private async void btnThamGia_Click(object sender, EventArgs e)
        {
            string roomId = txtRoomID.Text.Trim();

            if (string.IsNullOrEmpty(roomId))
            {
                MessageBox.Show("Vui lòng nhập Room ID.");
                return;
            }

            try
            {
                var db = FirestoreHelper.GetDb();
                bool joined = await RoomService.JoinRoomAsync(db, roomId, currentUser.Uid, currentUser.Username);

                if (!joined)
                {
                    MessageBox.Show("Không thể tham gia phòng. Có thể phòng không tồn tại hoặc đã bắt đầu/kết thúc.");
                    return;
                }

                this.Hide();
                // Mở form chờ người chơi khác (Prepare)
                if (mainform.prepare != null)
                    try { mainform.prepare.Close(); } catch { }
                mainform.prepare = new Prepare(mainform, currentUser, roomId);
                mainform.prepare.Show();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tham gia phòng: " + ex.Message);
            }
        }


        private void txtRoomID_TextChanged(object sender, EventArgs e)
        {

        }



        private async void btnTaoPhong_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy instance Firestore
                MessageBox.Show("UID tạo phòng: " + currentUser.Uid);

                var db = FirestoreHelper.GetDb();

                // Tạo phòng mới với bài học ngẫu nhiên
                string roomId = await RoomService.CreateRoomWithRandomLessonAsync(db, currentUser.Uid, currentUser.Username);


                // Ẩn form hiện tại
                this.Hide();

                // Mở form phòng chờ (Prepare)
                if (mainform.prepare != null)
                    try { mainform.prepare.Close(); } catch { }
                mainform.prepare = new Prepare(mainform, currentUser, roomId);
                mainform.prepare.Show();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo phòng: " + ex.Message);
            }
        }

        private async void btnGhepNgauNhien_Click(object sender, EventArgs e)
        {
            try
            {
                var db = FirestoreHelper.GetDb();

                string roomId = await RoomService.FindOrCreateAvailableRoomAsync(db, currentUser.Uid, currentUser.Username);

                // Ẩn form hiện tại
                this.Hide();

                // Mở form phòng chờ
                if (mainform.prepare != null)
                    try { mainform.prepare.Close(); } catch { }
                mainform.prepare = new Prepare(mainform, currentUser, roomId);
                mainform.prepare.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi ghép ngẫu nhiên: " + ex.Message);
            }
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            this.Hide();
            string callerName = "me";
            if (mainform.profile != null)
                try { mainform.profile.Close(); } catch { }
            mainform.profile = new Profile(mainform, currentUser, this, callerName);
            mainform.profile.Show();
            
        }
        private void btnForum_Click(object sender, EventArgs e)
        {
            this.Hide();
            string callerName = "no";
            mainform.forum = new Forum(mainform, currentUser, this, callerName);
            mainform.forum.Show();
            
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.home == null || mainform.home.IsDisposed)
            {
                if (mainform.home != null)
                    try { mainform.home.Close(); } catch { }
                mainform.home = new Home(mainform, currentUser);
            }
            mainform.home.Show();
            this.Close();
        }
        private void btnFlashcard_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.flashcardform != null)
                try { mainform.flashcardform.Close(); } catch { }
            mainform.flashcardform = new FlashcardForm(mainform, currentUser, currentUser.Uid);
            mainform.flashcardform.Show();
            this.Close();
        }
        private void Quiz_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.selecttopicform != null)
                try { mainform.selecttopicform.Close(); } catch { }
            mainform.selecttopicform = new SelectTopicForm(mainform, currentUser);
            mainform.selecttopicform.Show();
            this.Close();
        }
        private void Bangxephang_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.leaderboard != null)
                try { mainform.leaderboard.Close(); } catch { }
            mainform.leaderboard = new LeaderBoard(mainform, currentUser);
            mainform.leaderboard.Show();
            this.Close();
        }
        private void Thachdau_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.room != null)
                try { mainform.room.Close(); } catch { }
            mainform.room = new Room(mainform, currentUser);
            mainform.room.Show();
            this.Close();
        }
        private void Lichsu_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (mainform.history != null)
                try { mainform.history.Close(); } catch { }
            mainform.history = new History(mainform, currentUser);
            mainform.history.Show();
            this.Close();
        }
    }
}
