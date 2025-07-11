using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordUp.Models
{
    public class AnswerModel
    {
        public string Uid { get; set; }              // Người chơi
        public string Answer { get; set; }           // Đáp án đã chọn
        public int QuestionIndex { get; set; }       // Câu số mấy
        public bool IsCorrect { get; set; }          // Đúng/Sai
    }
}