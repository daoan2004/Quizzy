using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBase.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoreSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                SET IDENTITY_INSERT [Category] ON;
                INSERT INTO [Category] ([ID], [title], [description]) VALUES
                (105, N'Cloud Computing', N'Cloud services, deployment, and infrastructure basics'),
                (106, N'Cybersecurity', N'Security fundamentals and safer software practices'),
                (107, N'Algorithms', N'Data structures, complexity, and problem solving'),
                (108, N'Data Analytics', N'Data analysis, reporting, and practical statistics');
                SET IDENTITY_INSERT [Category] OFF;

                SET IDENTITY_INSERT [Subjects] ON;
                INSERT INTO [Subjects] ([ID], [UserID], [title], [brief_info], [Description], [rate], [isHot], [thumbnail_color], [registerDate]) VALUES
                (204, 10002, N'JavaScript Essentials', N'Master JavaScript syntax, DOM events, arrays, objects, and async basics.', N'A practical JavaScript subject for learners who want to build interactive web pages and prepare for frontend quizzes.', 5, 1, N'subject.jpg', '2026-06-25T09:00:00'),
                (205, 10002, N'Python for Data Analysis', N'Use Python, collections, files, and basic analysis patterns for real data tasks.', N'Learn Python fundamentals for data workflows, including lists, dictionaries, file handling, functions, and simple analysis habits.', 5, 1, N'subject.jpg', '2026-06-25T10:00:00'),
                (206, 10002, N'Cybersecurity Basics', N'Understand threats, authentication, hashing, secure coding, and network safety.', N'A beginner friendly security subject covering core concepts for safer systems and better technical quiz performance.', 4, 1, N'subject.jpg', '2026-06-25T11:00:00'),
                (207, 10002, N'Cloud Computing Foundations', N'Learn cloud models, storage, compute, networking, scaling, and deployment basics.', N'Cloud fundamentals for learners who need a clear map of infrastructure, services, scaling, reliability, and deployment concepts.', 4, 0, N'subject.jpg', '2026-06-25T12:00:00'),
                (208, 10002, N'Algorithms and Data Structures', N'Practice arrays, stacks, queues, trees, sorting, searching, and complexity.', N'A focused subject for algorithmic thinking with quiz practice around common structures, complexity, and problem solving patterns.', 5, 1, N'subject.jpg', '2026-06-25T13:00:00');
                SET IDENTITY_INSERT [Subjects] OFF;

                SET IDENTITY_INSERT [Price_package] ON;
                INSERT INTO [Price_package] ([ID], [SubjectID], [PackageType], [ListPrice], [SalePrice]) VALUES
                (310, 204, 1, 59, 39), (311, 204, 2, 99, 69), (312, 204, 3, 159, 109),
                (313, 205, 1, 69, 45), (314, 205, 2, 119, 79), (315, 205, 3, 189, 129),
                (316, 206, 1, 79, 49), (317, 206, 2, 139, 89), (318, 206, 3, 209, 149),
                (319, 207, 1, 69, 44), (320, 207, 2, 119, 82), (321, 207, 3, 199, 139),
                (322, 208, 1, 75, 49), (323, 208, 2, 129, 89), (324, 208, 3, 199, 139);
                SET IDENTITY_INSERT [Price_package] OFF;

                INSERT INTO [Subject_Category] ([SubjectID], [CategoryID]) VALUES
                (204, 101), (204, 103),
                (205, 101), (205, 108),
                (206, 106), (206, 104),
                (207, 105), (207, 103),
                (208, 107), (208, 104);

                SET IDENTITY_INSERT [Slider] ON;
                INSERT INTO [Slider] ([ID], [userID], [Title], [image], [backlink], [description], [status], [publishAt], [updatedAt]) VALUES
                (404, 10002, N'JavaScript Essentials', N'carousel-1.jpg', N'/Subjects/Details/204', N'Practice interactive web fundamentals with JavaScript quizzes.', 1, '2026-06-26T09:00:00', '2026-06-26T09:00:00'),
                (405, 10002, N'Python for Data Analysis', N'carousel-2.jpg', N'/Subjects/Details/205', N'Build confidence with Python data tasks.', 1, '2026-06-26T10:00:00', '2026-06-26T10:00:00'),
                (406, 10002, N'Cybersecurity Basics', N'scientitst.jpg', N'/Subjects/Details/206', N'Learn security concepts through clear quiz practice.', 1, '2026-06-26T11:00:00', '2026-06-26T11:00:00'),
                (407, 10002, N'Cloud Foundations', N'OpenSlide.jpg', N'/Subjects/Details/207', N'Explore cloud services, scaling, and deployment basics.', 1, '2026-06-26T12:00:00', '2026-06-26T12:00:00'),
                (408, 10002, N'Algorithms Practice', N'memoriam.jpg', N'/Subjects/Details/208', N'Sharpen problem solving with algorithm questions.', 1, '2026-06-26T13:00:00', '2026-06-26T13:00:00');
                SET IDENTITY_INSERT [Slider] OFF;

                SET IDENTITY_INSERT [Blogs] ON;
                INSERT INTO [Blogs] ([ID], [userID], [title], [body], [description], [status], [publishAt], [updatedAt], [blog_picture], [link_media], [url]) VALUES
                (504, 10002, N'Five Habits for Better Practice Sessions', N'Set a small goal, pick one topic, answer a limited set of questions, review mistakes, and repeat the next day.', N'Build a steady practice routine that is easier to maintain.', 1, '2026-06-19T09:00:00', '2026-06-19T09:00:00', N'carousel-1.jpg', N'', N'/Blogs/BlogsDetail?blogid=504&userid=10002'),
                (505, 10002, N'How to Review Wrong Answers', N'Reviewing wrong answers is most useful when you write down why the correct option works and why your selected option failed.', N'Turn mistakes into a clear study plan.', 1, '2026-06-20T09:00:00', '2026-06-20T09:00:00', N'carousel-2.jpg', N'', N'/Blogs/BlogsDetail?blogid=505&userid=10002'),
                (506, 10002, N'Frontend Quiz Checklist', N'Before a frontend quiz, revisit DOM events, layout rules, accessibility basics, async code, and browser debugging.', N'A compact checklist for frontend learners.', 1, '2026-06-21T09:00:00', '2026-06-21T09:00:00', N'streetplay.jpg', N'', N'/Blogs/BlogsDetail?blogid=506&userid=10002'),
                (507, 10002, N'SQL Joins Without Panic', N'Learn joins by drawing tables first, then matching the rows you expect before writing the query.', N'A calmer way to understand SQL joins.', 1, '2026-06-22T09:00:00', '2026-06-22T09:00:00', N'image1.jpg', N'', N'/Blogs/BlogsDetail?blogid=507&userid=10002'),
                (508, 10002, N'Security Terms Every Learner Should Know', N'Authentication, authorization, hashing, encryption, least privilege, and input validation appear often in security quizzes.', N'Core security vocabulary for quiz practice.', 1, '2026-06-23T09:00:00', '2026-06-23T09:00:00', N'scientitst.jpg', N'', N'/Blogs/BlogsDetail?blogid=508&userid=10002'),
                (509, 10002, N'Cloud Concepts in Plain Language', N'Cloud computing becomes easier when you separate compute, storage, networking, deployment, monitoring, and cost control.', N'A simple map for cloud fundamentals.', 1, '2026-06-24T09:00:00', '2026-06-24T09:00:00', N'OpenSlide.jpg', N'', N'/Blogs/BlogsDetail?blogid=509&userid=10002'),
                (510, 10002, N'Algorithm Practice That Actually Works', N'Start with the brute force idea, measure its cost, then improve one bottleneck at a time.', N'A practical approach to algorithm questions.', 1, '2026-06-25T09:00:00', '2026-06-25T09:00:00', N'memoriam.jpg', N'', N'/Blogs/BlogsDetail?blogid=510&userid=10002');
                SET IDENTITY_INSERT [Blogs] OFF;

                INSERT INTO [Blogs_Category] ([BlogID], [CategoryID]) VALUES
                (504, 104), (504, 101), (505, 104), (506, 103), (507, 102), (508, 106), (509, 105), (510, 107);

                SET IDENTITY_INSERT [SimulationExam] ON;
                INSERT INTO [SimulationExam] ([ID], [SubjectID], [LevelID], [ExamName], [Number_Question], [Duration], [Passrate]) VALUES
                (607, 204, 1, N'JavaScript Entry Simulation Exam', 10, 20, 60.00),
                (608, 204, 2, N'JavaScript Intermediate Simulation Exam', 15, 30, 70.00),
                (609, 205, 1, N'Python Data Entry Simulation Exam', 10, 20, 60.00),
                (610, 205, 2, N'Python Data Intermediate Simulation Exam', 15, 30, 70.00),
                (611, 206, 1, N'Security Entry Simulation Exam', 10, 20, 60.00),
                (612, 206, 2, N'Security Intermediate Simulation Exam', 15, 30, 70.00),
                (613, 207, 1, N'Cloud Entry Simulation Exam', 10, 20, 60.00),
                (614, 207, 2, N'Cloud Intermediate Simulation Exam', 15, 30, 70.00),
                (615, 208, 1, N'Algorithms Entry Simulation Exam', 10, 20, 60.00),
                (616, 208, 2, N'Algorithms Intermediate Simulation Exam', 15, 30, 70.00);
                SET IDENTITY_INSERT [SimulationExam] OFF;

                SET IDENTITY_INSERT [QuizBank] ON;
                INSERT INTO [QuizBank] ([ID], [SubjectID], [TopicID], [LevelID], [Status], [GroupID], [Title], [QA], [QB], [QC], [QD], [QE], [QF], [Qcorrect]) VALUES
                (1025, 204, 1, 1, 1, N'Choose one', N'Which keyword declares a block scoped variable in JavaScript?', N'var', N'let', N'def', N'int', N'', N'', N'B'),
                (1026, 204, 1, 1, 1, N'Choose one', N'Which method adds an item to the end of an array?', N'push', N'pop', N'shift', N'slice', N'', N'', N'A'),
                (1027, 204, 1, 1, 1, N'Choose one', N'Which API selects the first matching DOM element?', N'querySelector', N'fetchOne', N'getStyle', N'createNode', N'', N'', N'A'),
                (1028, 204, 1, 2, 1, N'Choose one', N'What does a Promise represent?', N'A future async result', N'A CSS rule', N'A database table', N'A browser cookie only', N'', N'', N'A'),
                (1029, 204, 1, 2, 1, N'Choose one', N'Which syntax is used for template literals?', N'Single quotes', N'Double quotes', N'Backticks', N'Angle brackets', N'', N'', N'C'),
                (1030, 204, 1, 2, 1, N'Choose one', N'Which event fires after HTML is parsed?', N'DOMContentLoaded', N'BeforeParse', N'CssReady', N'NodeDone', N'', N'', N'A'),
                (1031, 204, 1, 3, 1, N'Choose one', N'Which operator checks strict equality?', N'==', N'===', N'=', N'!=', N'', N'', N'B'),
                (1032, 204, 1, 3, 1, N'Choose one', N'Which function parses JSON text?', N'JSON.parse', N'JSON.read', N'Object.parseJson', N'Text.toJson', N'', N'', N'A'),
                (1033, 205, 1, 1, 1, N'Choose one', N'Which Python type stores key value pairs?', N'list', N'tuple', N'dict', N'set', N'', N'', N'C'),
                (1034, 205, 1, 1, 1, N'Choose one', N'Which keyword defines a function in Python?', N'func', N'def', N'function', N'lambdaonly', N'', N'', N'B'),
                (1035, 205, 1, 1, 1, N'Choose one', N'Which method reads all lines from a file object?', N'readlines', N'getrows', N'fetchlines', N'scan', N'', N'', N'A'),
                (1036, 205, 1, 2, 1, N'Choose one', N'Which structure removes duplicate values?', N'list', N'set', N'string', N'range', N'', N'', N'B'),
                (1037, 205, 1, 2, 1, N'Choose one', N'Which library is widely used for tabular data analysis?', N'pandas', N'turtle', N'flask', N'pytest', N'', N'', N'A'),
                (1038, 205, 1, 2, 1, N'Choose one', N'Which expression creates a list from an iterable compactly?', N'list comprehension', N'row map', N'class import', N'file tuple', N'', N'', N'A'),
                (1039, 205, 1, 3, 1, N'Choose one', N'Which exception often appears when a dictionary key is missing?', N'KeyError', N'NameErrorOnly', N'ValueMissing', N'RowError', N'', N'', N'A'),
                (1040, 205, 1, 3, 1, N'Choose one', N'Which statement handles cleanup after opening a file?', N'with', N'keep', N'usingfile', N'finallyonly', N'', N'', N'A'),
                (1041, 206, 1, 1, 1, N'Choose one', N'What is authentication?', N'Verifying identity', N'Compressing files', N'Deleting logs', N'Formatting disks', N'', N'', N'A'),
                (1042, 206, 1, 1, 1, N'Choose one', N'What should passwords usually be stored as?', N'Plain text', N'Hashed values', N'Comments', N'File names', N'', N'', N'B'),
                (1043, 206, 1, 1, 1, N'Choose one', N'Which attack injects malicious SQL into input?', N'SQL injection', N'Cross compile', N'Packet sort', N'Hash join', N'', N'', N'A'),
                (1044, 206, 1, 2, 1, N'Choose one', N'What is authorization?', N'Deciding allowed actions', N'Starting a server', N'Encrypting images', N'Running tests', N'', N'', N'A'),
                (1045, 206, 1, 2, 1, N'Choose one', N'Which principle gives users only needed permissions?', N'Least privilege', N'Most access', N'Fast grant', N'Open default', N'', N'', N'A'),
                (1046, 206, 1, 2, 1, N'Choose one', N'Which header helps reduce XSS risk by controlling allowed sources?', N'Content Security Policy', N'Accept Language', N'Cache Age', N'Host Name', N'', N'', N'A'),
                (1047, 206, 1, 3, 1, N'Choose one', N'Which token type is often used for stateless API authentication?', N'JWT', N'CSV', N'PNG', N'DDL', N'', N'', N'A'),
                (1048, 206, 1, 3, 1, N'Choose one', N'Which practice reduces brute force login risk?', N'Rate limiting', N'Longer CSS', N'Bigger images', N'Disable logs', N'', N'', N'A'),
                (1049, 207, 1, 1, 1, N'Choose one', N'Which cloud model provides virtual machines and networking?', N'IaaS', N'SaaS only', N'CSV', N'DOM', N'', N'', N'A'),
                (1050, 207, 1, 1, 1, N'Choose one', N'Which service type usually hosts complete software for users?', N'SaaS', N'IaaS only', N'RAM', N'DNS only', N'', N'', N'A'),
                (1051, 207, 1, 1, 1, N'Choose one', N'What does scaling mean in cloud systems?', N'Adjusting capacity', N'Changing font size', N'Deleting users', N'Writing CSS', N'', N'', N'A'),
                (1052, 207, 1, 2, 1, N'Choose one', N'Which concept spreads traffic across servers?', N'Load balancing', N'File locking', N'Query sorting', N'Package restore', N'', N'', N'A'),
                (1053, 207, 1, 2, 1, N'Choose one', N'Which storage is best for unstructured files like images?', N'Object storage', N'CPU cache', N'Process stack', N'Local variable', N'', N'', N'A'),
                (1054, 207, 1, 2, 1, N'Choose one', N'Which practice tracks application health over time?', N'Monitoring', N'Minifying only', N'Hardcoding', N'Ignoring logs', N'', N'', N'A'),
                (1055, 207, 1, 3, 1, N'Choose one', N'Which design improves availability across failures?', N'Redundancy', N'Single instance only', N'No backups', N'Hidden errors', N'', N'', N'A'),
                (1056, 207, 1, 3, 1, N'Choose one', N'Which approach deploys small changes frequently?', N'Continuous delivery', N'Manual yearly release', N'Offline only', N'No versioning', N'', N'', N'A'),
                (1057, 208, 1, 1, 1, N'Choose one', N'Which structure follows first in first out?', N'Queue', N'Stack', N'Tree', N'Heap only', N'', N'', N'A'),
                (1058, 208, 1, 1, 1, N'Choose one', N'Which structure follows last in first out?', N'Stack', N'Queue', N'Graph', N'Array only', N'', N'', N'A'),
                (1059, 208, 1, 1, 1, N'Choose one', N'Which algorithm finds an item in a sorted array by halving the search range?', N'Binary search', N'Linear append', N'Bubble write', N'Queue scan', N'', N'', N'A'),
                (1060, 208, 1, 2, 1, N'Choose one', N'What does Big O describe?', N'Growth of resource usage', N'Color of output', N'File size only', N'Network port', N'', N'', N'A'),
                (1061, 208, 1, 2, 1, N'Choose one', N'Which sort repeatedly selects the smallest remaining item?', N'Selection sort', N'Hash sort', N'Queue sort', N'Random sort', N'', N'', N'A'),
                (1062, 208, 1, 2, 1, N'Choose one', N'Which data structure is often used for recursion call tracking?', N'Stack', N'Queue', N'Set', N'Graph', N'', N'', N'A'),
                (1063, 208, 1, 3, 1, N'Choose one', N'Which traversal visits left subtree, node, then right subtree?', N'Inorder', N'Preorder only', N'Postorder only', N'Level skip', N'', N'', N'A'),
                (1064, 208, 1, 3, 1, N'Choose one', N'Which graph algorithm finds shortest paths with nonnegative weights?', N'Dijkstra', N'Bubble sort', N'Merge sort', N'Binary search', N'', N'', N'A');
                SET IDENTITY_INSERT [QuizBank] OFF;

                SET IDENTITY_INSERT [Recipe] ON;
                INSERT INTO [Recipe] ([ID], [PricePackage_ID], [UserID], [SubjectID], [PricePackage_Type], [BuyAt], [EndAt], [Status]) VALUES
                (704, 310, 10003, 204, 1, '2026-06-26T09:00:00', '2026-09-26T09:00:00', N'Registrated'),
                (705, 313, 10003, 205, 1, '2026-06-26T10:00:00', '2026-09-26T10:00:00', N'Registrated'),
                (706, 316, 10003, 206, 1, '2026-06-26T11:00:00', '2026-09-26T11:00:00', N'Submitted'),
                (707, 319, 10003, 207, 1, '2026-06-26T12:00:00', '2026-09-26T12:00:00', N'Registrated'),
                (708, 322, 10003, 208, 1, '2026-06-26T13:00:00', '2026-09-26T13:00:00', N'Registrated');
                SET IDENTITY_INSERT [Recipe] OFF;

                SET IDENTITY_INSERT [Practice] ON;
                INSERT INTO [Practice] ([ID], [UserID], [SubjectID], [title], [taken_date], [duration], [number_quest], [number_correct], [levelID], [topicID], [time_taken], [Quest_group], [Status]) VALUES
                (803, 10003, 204, N'JavaScript DOM Practice', '2026-06-26T14:00:00', '00:20:00', 5, 4, 1, 1, '00:13:20', N'Choose one', 1),
                (804, 10003, 205, N'Python Data Warm-up', '2026-06-26T15:00:00', '00:20:00', 5, 3, 1, 1, '00:14:05', N'Choose one', 1),
                (805, 10003, 206, N'Security Concepts Practice', '2026-06-26T16:00:00', '00:25:00', 5, 4, 2, 1, '00:17:40', N'Choose one', 1),
                (806, 10003, 207, N'Cloud Basics Practice', '2026-06-26T17:00:00', '00:25:00', 5, 4, 2, 1, '00:16:25', N'Choose one', 1),
                (807, 10003, 208, N'Algorithms Starter Practice', '2026-06-26T18:00:00', '00:30:00', 5, 3, 2, 1, '00:19:15', N'Choose one', 1);
                SET IDENTITY_INSERT [Practice] OFF;

                SET IDENTITY_INSERT [QuizHandle] ON;
                INSERT INTO [QuizHandle] ([ID], [UserID], [PracticeID], [QuizID], [QAnswer], [isMark], [status], [isCorrect]) VALUES
                (911, 10003, 803, 1025, N'B', 0, 1, 1), (912, 10003, 803, 1026, N'A', 0, 1, 1), (913, 10003, 803, 1027, N'A', 0, 1, 1), (914, 10003, 803, 1028, N'C', 0, 1, 0), (915, 10003, 803, 1029, N'C', 0, 1, 1),
                (916, 10003, 804, 1033, N'C', 0, 1, 1), (917, 10003, 804, 1034, N'B', 0, 1, 1), (918, 10003, 804, 1035, N'C', 0, 1, 0), (919, 10003, 804, 1036, N'B', 0, 1, 1), (920, 10003, 804, 1037, N'C', 0, 1, 0),
                (921, 10003, 805, 1041, N'A', 0, 1, 1), (922, 10003, 805, 1042, N'B', 0, 1, 1), (923, 10003, 805, 1043, N'A', 0, 1, 1), (924, 10003, 805, 1044, N'A', 0, 1, 1), (925, 10003, 805, 1045, N'B', 0, 1, 0),
                (926, 10003, 806, 1049, N'A', 0, 1, 1), (927, 10003, 806, 1050, N'A', 0, 1, 1), (928, 10003, 806, 1051, N'C', 0, 1, 0), (929, 10003, 806, 1052, N'A', 0, 1, 1), (930, 10003, 806, 1053, N'A', 0, 1, 1),
                (931, 10003, 807, 1057, N'A', 0, 1, 1), (932, 10003, 807, 1058, N'C', 0, 1, 0), (933, 10003, 807, 1059, N'A', 0, 1, 1), (934, 10003, 807, 1060, N'A', 0, 1, 1), (935, 10003, 807, 1061, N'C', 0, 1, 0);
                SET IDENTITY_INSERT [QuizHandle] OFF;
                """);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM [QuizHandle] WHERE [ID] BETWEEN 911 AND 935;
                DELETE FROM [Practice] WHERE [ID] BETWEEN 803 AND 807;
                DELETE FROM [Recipe] WHERE [ID] BETWEEN 704 AND 708;
                DELETE FROM [QuizBank] WHERE [ID] BETWEEN 1025 AND 1064;
                DELETE FROM [SimulationExam] WHERE [ID] BETWEEN 607 AND 616;
                DELETE FROM [Blogs_Category] WHERE [BlogID] BETWEEN 504 AND 510;
                DELETE FROM [Blogs] WHERE [ID] BETWEEN 504 AND 510;
                DELETE FROM [Slider] WHERE [ID] BETWEEN 404 AND 408;
                DELETE FROM [Subject_Category] WHERE [SubjectID] BETWEEN 204 AND 208;
                DELETE FROM [Price_package] WHERE [ID] BETWEEN 310 AND 324;
                DELETE FROM [Subjects] WHERE [ID] BETWEEN 204 AND 208;
                DELETE FROM [Category] WHERE [ID] BETWEEN 105 AND 108;
                """);

        }
    }
}
