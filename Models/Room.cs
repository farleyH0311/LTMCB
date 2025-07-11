using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordUp.Models
{
    public class RoomModel
    {
        public string RoomId { get; set; }           // Mã phòng 6 ký tự
        public string HostUid { get; set; }          // UID của người tạo phòng
        public bool Started { get; set; }            // Trạng thái đã bắt đầu chơi
        public bool Ended { get; set; }              // Trạng thái kết thúc
        public int CurrentQuestion { get; set; }     // Chỉ số câu hỏi hiện tại
        public Lesson Lesson { get; set; }           // Chủ đề và câu hỏi đã chọn
        public Dictionary<string, Player> Players { get; set; } // Danh sách người chơi
    }
}