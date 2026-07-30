using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBase.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                SET IDENTITY_INSERT [Users] ON;
                INSERT INTO [Users] ([ID], [email], [fullname], [password], [address], [Phone], [gender], [Dob], [RoleID], [profile_picture], [register_date], [description], [status], [verificationToken], [PasswordResetToken], [PasswordResetTokenExpires]) VALUES
                (10001, N'admin@quizzy.local', N'Admin Quizzy', N'AQAAAAIAAYagAAAAEG6IpzLmHwxkU27MH5fOF0fu9Ua7apZ1JkqbJR+BKy/wK3hybXoMJoCxtt0WnwB+Xw==', N'Ho Chi Minh City', N'0900000001', 1, '1995-01-01', 1, NULL, '2026-06-01T08:00:00', N'Sample admin account', 1, NULL, NULL, NULL),
                (10002, N'marketing@quizzy.local', N'Marketing Quizzy', N'AQAAAAIAAYagAAAAEA3OsRb27GV+Nf9OHyvQ1K5z9kRkVz2XP2kplqxSd//rS33/JRBbhdFTSj+4G5Jp1w==', N'Ho Chi Minh City', N'0900000002', 0, '1996-02-02', 3, NULL, '2026-06-02T08:00:00', N'Sample marketing account', 1, NULL, NULL, NULL),
                (10003, N'customer@quizzy.local', N'Customer Quizzy', N'AQAAAAIAAYagAAAAEOt3jQSy8WjVvNR9q7rBR6we/jO7o4/KHfZ+tNoytfXvmWG1WZfcm741619vf5E11A==', N'Ha Noi', N'0900000003', 1, '2000-03-03', 2, NULL, '2026-06-03T08:00:00', N'Sample customer account', 1, NULL, NULL, NULL);
                SET IDENTITY_INSERT [Users] OFF;

                SET IDENTITY_INSERT [Category] ON;
                INSERT INTO [Category] ([ID], [title], [description]) VALUES
                (101, N'Programming', N'Programming and software engineering'),
                (102, N'Database', N'Database design and SQL practice'),
                (103, N'Web Development', N'Frontend and backend web development'),
                (104, N'Computer Science', N'Computer science foundations');
                SET IDENTITY_INSERT [Category] OFF;

                SET IDENTITY_INSERT [Subjects] ON;
                INSERT INTO [Subjects] ([ID], [UserID], [title], [brief_info], [Description], [rate], [isHot], [thumbnail_color], [registerDate]) VALUES
                (201, 10002, N'C# Fundamentals', N'Learn C# syntax, OOP, LINQ, and practical problem solving.', N'A beginner friendly C# course with quizzes for syntax, classes, collections, LINQ, and exception handling.', 5, 1, N'subject.jpg', '2026-06-10T09:00:00'),
                (202, 10002, N'ASP.NET Core MVC', N'Build web apps with controllers, views, routing, and EF Core.', N'Hands-on ASP.NET Core MVC subject covering routing, Razor views, model binding, validation, authentication basics, and EF Core.', 5, 1, N'subject.jpg', '2026-06-11T09:00:00'),
                (203, 10002, N'SQL Server Essentials', N'Practice relational design, joins, indexes, and transactions.', N'SQL Server training for database fundamentals, querying, normalization, joins, stored procedures, and query performance.', 4, 1, N'subject.jpg', '2026-06-12T09:00:00');
                SET IDENTITY_INSERT [Subjects] OFF;

                SET IDENTITY_INSERT [Price_package] ON;
                INSERT INTO [Price_package] ([ID], [SubjectID], [PackageType], [ListPrice], [SalePrice]) VALUES
                (301, 201, 1, 59, 39), (302, 201, 2, 99, 69), (303, 201, 3, 159, 109),
                (304, 202, 1, 69, 49), (305, 202, 2, 119, 79), (306, 202, 3, 189, 129),
                (307, 203, 1, 49, 29), (308, 203, 2, 89, 59), (309, 203, 3, 139, 99);
                SET IDENTITY_INSERT [Price_package] OFF;

                INSERT INTO [Subject_Category] ([SubjectID], [CategoryID]) VALUES
                (201, 101), (201, 104), (202, 101), (202, 103), (203, 102), (203, 104);

                SET IDENTITY_INSERT [Slider] ON;
                INSERT INTO [Slider] ([ID], [userID], [Title], [image], [backlink], [description], [status], [publishAt], [updatedAt]) VALUES
                (401, 10002, N'C# Fundamentals', N'image1.jpg', N'/Subjects/Details/201', N'Start learning C# with guided quizzes.', 1, '2026-06-13T09:00:00', '2026-06-13T09:00:00'),
                (402, 10002, N'ASP.NET Core MVC', N'image2.jpg', N'/Subjects/Details/202', N'Practice MVC and EF Core through real questions.', 1, '2026-06-14T09:00:00', '2026-06-14T09:00:00'),
                (403, 10002, N'SQL Server Essentials', N'image3.jpg', N'/Subjects/Details/203', N'Build confidence in SQL queries and schema design.', 1, '2026-06-15T09:00:00', '2026-06-15T09:00:00');
                SET IDENTITY_INSERT [Slider] OFF;

                SET IDENTITY_INSERT [Blogs] ON;
                INSERT INTO [Blogs] ([ID], [userID], [title], [body], [description], [status], [publishAt], [updatedAt], [blog_picture], [link_media], [url]) VALUES
                (501, 10002, N'How to Prepare for Technical Quizzes', N'Plan your study time, review fundamentals, and practice small sets of questions every day.', N'A practical guide for quiz preparation.', 1, '2026-06-16T09:00:00', '2026-06-16T09:00:00', N'blog-web.jpg', N'', N'/Blogs/BlogsDetail?blogid=501&userid=10002'),
                (502, 10002, N'Why Practice History Matters', N'Looking back at previous attempts helps you find weak topics and improve faster.', N'Use practice history to learn smarter.', 1, '2026-06-17T09:00:00', '2026-06-17T09:00:00', N'OpenSlide.jpg', N'', N'/Blogs/BlogsDetail?blogid=502&userid=10002'),
                (503, 10002, N'Choosing the Right Subject Package', N'Short packages are good for refreshers; longer packages fit deep learning plans.', N'Compare packages before registering.', 1, '2026-06-18T09:00:00', '2026-06-18T09:00:00', N'FSlide.png', N'', N'/Blogs/BlogsDetail?blogid=503&userid=10002');
                SET IDENTITY_INSERT [Blogs] OFF;

                INSERT INTO [Blogs_Category] ([BlogID], [CategoryID]) VALUES
                (501, 101), (501, 104), (502, 104), (503, 103);

                SET IDENTITY_INSERT [SimulationExam] ON;
                INSERT INTO [SimulationExam] ([ID], [SubjectID], [LevelID], [ExamName], [Number_Question], [Duration], [Passrate]) VALUES
                (601, 201, 1, N'C# Entry Simulation Exam', 10, 20, 60.00),
                (602, 201, 2, N'C# Intermediate Simulation Exam', 15, 30, 70.00),
                (603, 202, 1, N'MVC Entry Simulation Exam', 10, 20, 60.00),
                (604, 202, 2, N'MVC Intermediate Simulation Exam', 15, 30, 70.00),
                (605, 203, 1, N'SQL Entry Simulation Exam', 10, 20, 60.00),
                (606, 203, 2, N'SQL Intermediate Simulation Exam', 15, 30, 70.00);
                SET IDENTITY_INSERT [SimulationExam] OFF;

                SET IDENTITY_INSERT [QuizBank] ON;
                INSERT INTO [QuizBank] ([ID], [SubjectID], [TopicID], [LevelID], [Status], [GroupID], [Title], [QA], [QB], [QC], [QD], [QE], [QF], [Qcorrect]) VALUES
                (1001, 201, 1, 1, 1, N'Choose one', N'Which keyword declares a variable whose type is inferred?', N'var', N'let', N'dim', N'auto', N'', N'', N'A'),
                (1002, 201, 1, 1, 1, N'Choose one', N'Which type stores true or false values in C#?', N'int', N'bool', N'string', N'decimal', N'', N'', N'B'),
                (1003, 201, 1, 1, 1, N'Choose one', N'Which symbol starts a single-line comment in C#?', N'#', N'--', N'//', N'/*', N'', N'', N'C'),
                (1004, 201, 1, 2, 1, N'Choose one', N'Which feature lets a class provide multiple methods with the same name but different parameters?', N'Inheritance', N'Encapsulation', N'Overloading', N'Boxing', N'', N'', N'C'),
                (1005, 201, 1, 2, 1, N'Choose one', N'Which LINQ method filters a sequence?', N'Select', N'Where', N'OrderBy', N'GroupBy', N'', N'', N'B'),
                (1006, 201, 1, 2, 1, N'Choose one', N'Which block always runs after try/catch if present?', N'using', N'final', N'finally', N'lock', N'', N'', N'C'),
                (1007, 201, 1, 3, 1, N'Choose one', N'Which construct releases IDisposable resources automatically?', N'using', N'await', N'yield', N'params', N'', N'', N'A'),
                (1008, 201, 1, 3, 1, N'Choose one', N'Which modifier prevents a class from being inherited?', N'private', N'sealed', N'static', N'abstract', N'', N'', N'B'),
                (1009, 202, 1, 1, 1, N'Choose one', N'In MVC, which component receives HTTP requests first?', N'Model', N'View', N'Controller', N'Migration', N'', N'', N'C'),
                (1010, 202, 1, 1, 1, N'Choose one', N'Razor view files usually use which extension?', N'.html', N'.cshtml', N'.razorclass', N'.mvc', N'', N'', N'B'),
                (1011, 202, 1, 1, 1, N'Choose one', N'Which method registers MVC controllers and views?', N'AddControllersWithViews', N'UseStaticFiles', N'AddDbContext', N'UseRouting', N'', N'', N'A'),
                (1012, 202, 1, 2, 1, N'Choose one', N'Which EF Core method loads related data eagerly?', N'Include', N'Attach', N'SaveChanges', N'Migrate', N'', N'', N'A'),
                (1013, 202, 1, 2, 1, N'Choose one', N'Which file commonly stores connection strings in ASP.NET Core?', N'launch.json', N'appsettings.json', N'package.json', N'web.config only', N'', N'', N'B'),
                (1014, 202, 1, 2, 1, N'Choose one', N'Which middleware enables authentication?', N'UseAuthentication', N'UseAuthorizationOnly', N'UseEndpoints', N'UseMvcData', N'', N'', N'A'),
                (1015, 202, 1, 3, 1, N'Choose one', N'Which attribute restricts access to authenticated users?', N'ValidateAntiForgeryToken', N'Authorize', N'Bind', N'RouteOnly', N'', N'', N'B'),
                (1016, 202, 1, 3, 1, N'Choose one', N'Which service configures EF Core SQL Server?', N'UseSqlServer', N'UseKestrelOnly', N'AddRazorRuntime', N'UseSqlClientView', N'', N'', N'A'),
                (1017, 203, 1, 1, 1, N'Choose one', N'Which SQL clause filters rows?', N'ORDER BY', N'WHERE', N'GROUP BY', N'JOIN', N'', N'', N'B'),
                (1018, 203, 1, 1, 1, N'Choose one', N'Which key uniquely identifies a row?', N'Foreign key', N'Primary key', N'Index hint', N'Check key', N'', N'', N'B'),
                (1019, 203, 1, 1, 1, N'Choose one', N'Which command reads data from a table?', N'SELECT', N'UPDATE', N'DELETE', N'ALTER', N'', N'', N'A'),
                (1020, 203, 1, 2, 1, N'Choose one', N'Which join returns matching rows from both tables?', N'LEFT JOIN', N'RIGHT JOIN', N'INNER JOIN', N'CROSS JOIN', N'', N'', N'C'),
                (1021, 203, 1, 2, 1, N'Choose one', N'Which aggregate counts rows?', N'SUM', N'AVG', N'COUNT', N'MIN', N'', N'', N'C'),
                (1022, 203, 1, 2, 1, N'Choose one', N'Which normal form reduces repeating groups?', N'1NF', N'2NF', N'3NF', N'BCNF only', N'', N'', N'A'),
                (1023, 203, 1, 3, 1, N'Choose one', N'Which object can improve read performance but may slow writes?', N'Index', N'View only', N'Cursor', N'Trigger only', N'', N'', N'A'),
                (1024, 203, 1, 3, 1, N'Choose one', N'Which isolation issue allows reading uncommitted data?', N'Dirty read', N'Deadlock', N'Rollback', N'Checkpoint', N'', N'', N'A');
                SET IDENTITY_INSERT [QuizBank] OFF;

                SET IDENTITY_INSERT [Recipe] ON;
                INSERT INTO [Recipe] ([ID], [PricePackage_ID], [UserID], [SubjectID], [PricePackage_Type], [BuyAt], [EndAt], [Status]) VALUES
                (701, 301, 10003, 201, 1, '2026-06-20T09:00:00', '2026-09-20T09:00:00', N'Registrated'),
                (702, 304, 10003, 202, 1, '2026-06-21T09:00:00', '2026-09-21T09:00:00', N'Registrated'),
                (703, 307, 10003, 203, 1, '2026-06-22T09:00:00', '2026-09-22T09:00:00', N'Submitted');
                SET IDENTITY_INSERT [Recipe] OFF;

                SET IDENTITY_INSERT [Practice] ON;
                INSERT INTO [Practice] ([ID], [UserID], [SubjectID], [title], [taken_date], [duration], [number_quest], [number_correct], [levelID], [topicID], [time_taken], [Quest_group], [Status]) VALUES
                (801, 10003, 201, N'C# Warm-up Practice', '2026-06-23T10:00:00', '00:20:00', 5, 3, 1, 1, '00:12:30', N'Choose one', 1),
                (802, 10003, 202, N'MVC Routing Practice', '2026-06-24T10:00:00', '00:25:00', 5, 4, 2, 1, '00:15:10', N'Choose one', 1);
                SET IDENTITY_INSERT [Practice] OFF;

                SET IDENTITY_INSERT [QuizHandle] ON;
                INSERT INTO [QuizHandle] ([ID], [UserID], [PracticeID], [QuizID], [QAnswer], [isMark], [status], [isCorrect]) VALUES
                (901, 10003, 801, 1001, N'A', 0, 1, 1), (902, 10003, 801, 1002, N'B', 0, 1, 1),
                (903, 10003, 801, 1003, N'A', 0, 1, 0), (904, 10003, 801, 1004, N'C', 0, 1, 1),
                (905, 10003, 801, 1005, N'A', 0, 1, 0), (906, 10003, 802, 1009, N'C', 0, 1, 1),
                (907, 10003, 802, 1010, N'B', 0, 1, 1), (908, 10003, 802, 1011, N'A', 0, 1, 1),
                (909, 10003, 802, 1012, N'A', 0, 1, 1), (910, 10003, 802, 1013, N'C', 0, 1, 0);
                SET IDENTITY_INSERT [QuizHandle] OFF;
                """);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM [QuizHandle] WHERE [ID] BETWEEN 901 AND 910;
                DELETE FROM [Practice] WHERE [ID] IN (801, 802);
                DELETE FROM [Recipe] WHERE [ID] IN (701, 702, 703);
                DELETE FROM [QuizBank] WHERE [ID] BETWEEN 1001 AND 1024;
                DELETE FROM [SimulationExam] WHERE [ID] BETWEEN 601 AND 606;
                DELETE FROM [Blogs_Category] WHERE [BlogID] BETWEEN 501 AND 503;
                DELETE FROM [Blogs] WHERE [ID] BETWEEN 501 AND 503;
                DELETE FROM [Slider] WHERE [ID] BETWEEN 401 AND 403;
                DELETE FROM [Subject_Category] WHERE [SubjectID] BETWEEN 201 AND 203;
                DELETE FROM [Price_package] WHERE [ID] BETWEEN 301 AND 309;
                DELETE FROM [Subjects] WHERE [ID] BETWEEN 201 AND 203;
                DELETE FROM [Category] WHERE [ID] BETWEEN 101 AND 104;
                DELETE FROM [Users] WHERE [ID] BETWEEN 10001 AND 10003;
                """);
        }
    }
}
