# -n-LTMCB-_-NT106.P22.ANTT-2025
```
1. Sơ đồ thư mục
WordUp!/
├── Form/
│   ├── WelcomeForm              # Trang chào mừng
│   ├── RegisterUI               # Đăng ký tài khoản (Firebase Auth)
│   ├── LoginForm                # Đăng nhập (Firebase Auth)
│   ├── Home                     # Trang chủ (thông tin tổng quát, tiến độ học tập)
│   ├── ForgotPassword           # Quên mật khẩu 
│   ├── Flashcard                # Chọn bài học và học bằng flashcard
│   ├── LeaderBoard              # Bảng xếp hạng
│   ├── PractiseQuiz             # Ôn tập quiz
│   ├── Forum                    # Diễn đàn thảo luận (đăng bài, bình luận, cảm xúc)
│   ├── Notification             # Thông báo về bài đăng, lời mời thách đấu quiz
│   ├── Profile                  # Hồ sơ cá nhân (hiển thị và cập nhật thông tin người dùng)
│   ├── Room                     # Thách đấu quiz real-time
│   ├── SelectTopicForm          # Chọn bài ôn tập
│   ├── CreateFlashcardForm      # Tạo bộ flashcard cá nhân (tự chế tạo từ vựng mới) -có thể chọn để bổ sung nếu cần tìm thêm tính năng
│   ├── LeaderboardForm          # Bảng xếp hạng (top người học, top quiz thủ)
│   ├── FriendListForm           # Quản lý bạn bè (kết bạn, tìm kiếm bạn bè, mời thách đấu)
│   ├── 
│   ├── FeedbackForm             # Gửi feedback cho ứng dụng
│   ├── ReportForm               # Báo cáo vi phạm (nội dung diễn đàn, người dùng vi phạm)
│   ├── SearchForm               # Tìm kiếm bài học, bài post diễn đàn, người dùng
│   └── HelpCenterForm           # Trung tâm trợ giúp (FAQ, hướng dẫn sử dụng)

│
├── Models/                      # Các model dữ liệu
│   ├── User                     # Dữ liệu người dùng (thông tin cá nhân, email, mật khẩu, UID từ Firebase)
│   ├── Post                     # Dữ liệu bài đăng trên diễn đàn
│   ├── Comment                  # Dữ liệu bình luận trên bài đăng
│   ├── Notification             # Dữ liệu thông báo
│   ├── Lesson                   # Dữ liệu quiz (bao gồm câu hỏi, đáp án, v.v.)
│   ├── Flashcard                # Dữ liệu bài học
│
├── Service/                              # Các dịch vụ hỗ trợ
│   ├── EmailService                      # Gửi email (xác nhận đăng ký, quên mật khẩu)
│   ├── PasswordHelper                    # Hỗ trợ mật khẩu (mã hóa mật khẩu, kiểm tra mật khẩu mạnh)
│   ├── FirebaseService                   # Tương tác với Firebase (auth, firestore, realtime database)
│   │   ├── FirebaseLessonService         # Quản lý dữ liệu collection "lessons" trong Firestore
│   │   └── FirebaseProgressService       # Quản lý tiến độ học tập, theo dõi bài học đã học
│   ├── FirestoreService                  # Quản lý dữ liệu Firebase (auth, firestore, realtime database)
│   │   ├── FirebaseFlashcardService      # Quản lý thao tác với collection "flashcard" trong Firestore
│   ├── CloudinaryService                 # Cung cấp dịch vụ upload ảnh
│   ├── FlashcardUploader                 # Quản lý dữ liệu collection "flashcard"
│   ├── LeadeBoardService                 # Quản lý dữ liệu bảng xếp hạng
│   ├── LessonUploader                    # Quản lý lesson trong quiz
│   └── SpeechService                     # Quản lý tiến độ học tập, theo dõi bài học đã học
│
```

## 🚀 Hướng dẫn chạy dự án
1. Tạo thư mục `/Resources` trong gốc dự án.
2. Tải file `serviceAccountKey.json` từ Firebase Console và bỏ vô `/Resources`. (Tui có upload sẵn có thể tại về trực tiếp nhưng nó ko dc bảo mật)
3. (Tuỳ chọn) Tải `lessons.json` và dùng tính năng upload nếu  muốn load dữ liệu lên thêm.
4. cài các gói NuGet:
   - `Google.Cloud.Firestore`
   - `Newtonsoft.Json`
   - `Guna.UI2.WinForms`
   - `System.Speech`
   - `NAudio`
5. Build và chạy project.

