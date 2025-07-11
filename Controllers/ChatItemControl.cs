using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordUp.Controllers
{
    public partial class ChatItemControl : UserControl
    {
        public string Username { get; private set; }

        public event EventHandler<string> OnChatSelected;

        public ChatItemControl(string displayName, string avatarPath, string username)
        {
            InitializeComponent();
            Username = username;
            lblName.Text = displayName;
            picAvatar.ImageLocation = avatarPath;
            picAvatar.SizeMode = PictureBoxSizeMode.Zoom;

            this.Click += (s, e) => OnChatSelected?.Invoke(this, username);
            lblName.Click += (s, e) => OnChatSelected?.Invoke(this, username);
            picAvatar.Click += (s, e) => OnChatSelected?.Invoke(this, username);
        }
        public void SetSelected(bool isSelected)
        {
            this.BackColor = isSelected ? Color.LightGray : SystemColors.Control;
        }
    }

}
