# Lập trình mạng căn bản - NT106.P22.ANTT

**Ứng dụng:** *Vui học tiếng Anh cùng WORDUP*

Với **WordUp**, học tiếng Anh chưa bao giờ dễ dàng và thú vị đến thế!  
Ứng dụng hoàn toàn miễn phí, giúp bạn ghi nhớ từ vựng hiệu quả, luyện phát âm chuẩn, theo dõi tiến trình mỗi ngày và biến việc học trở thành niềm vui.

---

## Các Tính Năng Nổi Bật của WordUp

- **Tạo tài khoản dễ dàng & an toàn**  
  Đăng ký và đăng nhập nhanh chóng, bảo mật thông tin người dùng.

- **Học với flashcard đa dạng chủ đề**  
  Cung cấp nhiều chủ đề từ vựng phổ biến, đáng tin cậy giúp bạn ghi nhớ hiệu quả.

- **Luyện phát âm với công cụ chấm điểm**  
  Tự ghi âm, nhận điểm đánh giá độ chính xác, lưu loát trong cách phát âm.

- **Luyện nói tiếng Anh theo chủ đề**  
   Chọn chủ đề thỏa thích và ghi âm câu trả lời. AI nhận nhiệm vụ phân tích phát âm, ngữ pháp, gợi ý cách diễn đạt tự nhiên hơn.

- **Hỏi đáp cùng AI**  
  Đặt câu hỏi, học thêm ví dụ, ngữ cảnh hoặc giải nghĩa chi tiết với trợ lý AI thông minh.

- **Tra từ điển Anh - Việt nhanh chóng, chính xác**  
  Cung cấp nghĩa, phát âm ngay trong ứng dụng.

- **Ôn tập từ vựng hiệu quả**  
  Làm quiz ôn tập ngay sau bài học giúp ghi nhớ lâu hơn.

- **Thách đấu kiến thức cùng bạn bè**  
  Cạnh tranh điểm số với người chơi khác để tăng động lực học tập.

- **Lưu điểm và bảng xếp hạng**  
  Ghi lại tiến trình học và hiển thị thứ hạng theo tuần/tháng.

- **Cộng đồng học tập năng động**  
  Đăng bài chia sẻ, bình luận, thả cảm xúc và cùng nhau phát triển.

- **Nhắn tin, kết bạn**  
  Kết nối bạn bè, trao đổi kinh nghiệm và học tập cùng nhau dễ dàng.

---

## Công Nghệ Sử Dụng

- C# WinForms
- Firebase Authentication
- Firebase Realtime Database
- Firestore
- Azure Speech-to-Text (Microsoft Cognitive Services)
- Gemini AI (Google Generative Language API)
- Cloudinary
- Google Translate API

---
## Sơ đồ thư mục
```text
WordUp!/
├── Form/
│   ├── WelcomeForm              # Trang chào mừng
│   ├── RegisterUI               # Đăng ký tài khoản 
│   ├── LoginForm                # Đăng nhập 
│   ├── Home                     # Trang chủ (thông tin chung, tra từ, trò chuyện với AI)
│   ├── ForgotPassword           # Quên mật khẩu 
│   ├── Chat                     # Nhắn tin
│   ├── Game
│   ├── History                  # Lịch sử thi đấu
│   ├── UploadQuiz
│   ├── Prepare                  # Phòng chờ thách đấu
│   ├── Flashcard                # Chọn bài học và học bằng flashcard
│   ├── LeaderBoard              # Bảng xếp hạng
│   ├── PractiseQuiz             # Ôn tập quiz
│   ├── ResultForm               # Điểm thi đấu giữa các thành viên của phòng
│   ├── Forum                    # Diễn đàn thảo luận (đăng bài, bình luận, thả cảm xúc)
│   ├── Notification             # Thông báo về bài đăng, lời mời thách đấu quiz
│   ├── Profile                  # Hồ sơ cá nhân (hiển thị và chỉnh sửa thông tin người dùng)
│   ├── Room                     # Thách đấu quiz real-time
│   ├── SelectTopicForm          # Chọn bài ôn tập
│   └── LeaderboardForm          # Bảng xếp hạng (top người học, top quiz thủ)
│  
├── Models/                      # Các model dữ liệu
│   ├── User                     # Dữ liệu người dùng (thông tin cá nhân)
│   ├── Post                     # Dữ liệu bài đăng trên diễn đàn
│   ├── Comment                  # Dữ liệu bình luận trên bài đăng
│   ├── Notification             # Dữ liệu thông báo
│   ├── Lesson                   # Dữ liệu quiz (bao gồm câu hỏi, đáp án, v.v.)
│   ├── Flashcard                # Dữ liệu bài học
│   ├── Answer                   # Dữ liệu đáp án người chơi chọn
│   ├── Message                  # Dữ liệu tin nhắn
│   ├── Player                   # Dữ liệu người chơi Quiz thách đấu
│   ├── RecentChatInfo           # Dữ liệu chi tiết tin nhắn
│   └── Room                     # Thông tin phòng game
│
├── Service/                              # Các dịch vụ hỗ trợ
│   ├── AI                                # Gọi API AI
│   ├── EmailService                      # Gửi email (xác nhận đăng ký, quên mật khẩu)
│   ├── PasswordHelper                    # Hỗ trợ mật khẩu (mã hóa mật khẩu, kiểm tra mật khẩu mạnh)
│   ├── Firebase                          # Tương tác với Firebase (auth, firestore, realtime database)
│   │     ├── FirebaseHelper              # Quản lý danh sách các cuộc trò chuyện
│   │     ├── FirebaseService             # Quản lý tương tác với firebase realtime (tài khoản, bài đăng, tin nhắn...)
│   │     ├── FirebaseLessonService       # Quản lý dữ liệu collection "lessons" trong Firestore
│   │     ├── FirebaseProgressService     # Quản lý tiến độ học tập, theo dõi bài học đã học
│   │     ├── FirestoreHelper             # Kết nối với firestore
│   │     ├── FirestoreService            # Quản lý việc tải dữ liệu bài học lên firestore
│   │     └── FirestoreFlashcardService   # Quản lý flashcard
│   │ 
│   ├── CloudinaryService                 # Quản lý tải, hiển thị ảnh người dùng
│   ├── FlashcardUploader                 # Quản lý dữ liệu collection "flashcard"
│   ├── LeadeBoardService                 # Quản lý dữ liệu bảng xếp hạng
│   ├── LessonUploader                    # Quản lý lesson trong quiz
│   ├── RoomService.cs                    # Quản lý game thách đấu
│   └── SpeechService                     # Hỗ trợ phát âm
│
├── Controllers/  
│   ├── ChatItemControl                   # Hỗ trợ giao diện nhắn tin
│   ├── MessageBubble.cs                  # Hỗ trợ giao diện nhắn tin
│   ├── NotificationBubble                # Hỗ trợ giao diện thông báo
│   └── PostBubble                        # Hỗ trợ giao diện bài đăng

```
---
## Hướng dẫn chạy dự án
### Bước 1: Clone dự án từ GitHub
Mở terminal hoặc PowerShell và chạy lệnh:
```bash
git clone https://github.com/younglttlefrog/-n-LTMCB-_-NT106.P22.ANTT-2025.git
```

### Bước 2: Mở project bằng Visual Studio
- Dùng Visual Studio 2022 
- Mở file `.sln` trong thư mục đã clone

### Bước 3: Cài đặt các gói NuGet cần thiết
Vào `Tools > NuGet Package Manager > Manage NuGet Packages for Solution`  
Hoặc dùng `Package Manager Console` để cài lần lượt:
```powershell
Install-Package Google.Cloud.Firestore
Install-Package Newtonsoft.Json
Install-Package System.Speech
Install-Package Microsoft.CognitiveServices.Speech
Install-Package System.Net.Http
Install-Package Guna.UI2.WinForms
Install-Package NAudio
```

### Bước 4: Chuẩn Bị Dữ Liệu (Tùy chọn)
Có thể tải lên bài học tùy ý.
- Nếu có file lessons.json đặt vào thư mục /Resources.
- Mở ứng dụng và sử dụng tính năng Upload Lessons để nạp dữ liệu vào Firestore.

### Bước 5: Build & Run
- Nhấn **F5** hoặc chọn **Start** để chạy ứng dụng
- Giao diện WordUp sẽ hiển thị với các chức năng tương ứng.

---
## Thành viên & Đóng góp
Cả nhóm: 
- Chọn đề tài, công nghệ, hướng phát triển.
- Chọn các chức năng cần có của ứng dụng.
- Lên kế hoạch, thời gian làm việc.
- Kiểm thử và chỉnh sửa.
  
*Nhánh chính của dự án là **final** tuy nhiên sau khi merge các nhánh và sửa lỗi đã mất commit của các thành viên. Do đó phiền mọi người vui lòng xem đóng góp chi tiết của các thành viên qua nhánh **main** và **new**. Xin chân thành cảm ơn.*

### 1. **Kim Thái Vi Anh** – `23520045`– [`@younglttlefrog`](https://github.com/younglttlefrog)
- Thiết kế giao diện:
  - Trang đăng nhập, đăng ký.
  - Hướng dẫn sử dụng.
  - Trang quiz ôn tập.
  - Trang quiz thách đấu.
  - Trang bảng xếp hạng.
- Code chức năng:
  - Ôn tập quiz.
  - Bảng xếp hạng.
  - Thách đấu.
- Chỉnh sửa luồng hoạt động các form, tham gia merge code.
---

### 2. **Mai Thị Quỳnh Châu** – `23520170`– [`@oobbooz`](https://github.com/oobbooz)
- Thiết kế giao diện:
  - Trang quên mật khẩu.
  - Trang chủ.
  - Trang cá nhân.
  - Trang diễn đàn.
  - Trang tin nhắn.
  - Trang thông báo.
- Code chức năng:
  - Đăng nhập, đăng ký, quên mật khẩu, đăng xuất.
  - Hỏi đáp với AI.
  - Tra từ điển Anh Việt.
  - Tạo, chỉnh sửa hồ sơ cá nhân.
  - Diễn đàn đăng bài, tương tác.
  - Nhắn tin giữa người dùng.
  - Thông báo.
- Chỉnh sửa giao diện chung.
---

### 3. **Phạm Võ Khánh Hà** – `23520414`– [`@verstecktH0311`](https://github.com/verstecktH0311)
- Thiết kế giao diện:
  - Trang flashcard.
  - Trang chủ.
- Code chức năng:
  - Học từ vựng và phát âm với flashcard.
  - Kiểm tra phát âm và chấm điểm.
  - Luyện tập speaking với AI.
- Merge các branch của các thành viên trong git.



