using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyThuVien.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ISBN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PublishYear = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "int", nullable: false),
                    CoverImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Active", "Description", "Image", "Name" },
                values: new object[,]
                {
                    { 1, true, "Sách chuyên ngành Lập trình C#, ASP.NET Core, Cơ sở dữ liệu", null, "Công nghệ thông tin" },
                    { 2, true, "Sách Quản trị kinh doanh, Tài chính doanh nghiệp", null, "Kinh tế & Khởi nghiệp" },
                    { 3, true, "Tiểu thuyết, truyện ngắn, tác phẩm kinh điển", null, "Văn học & Nghệ thuật" },
                    { 4, true, "Phát triển bản thân, tư duy lãnh đạo, giao tiếp", null, "Kỹ năng sống" },
                    { 5, true, "Toán học, Vật lý, Thiên văn học", null, "Khoa học cơ bản" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Active", "Email", "EmployeeCode", "FullName", "Password", "Phone", "Role" },
                values: new object[,]
                {
                    { 1, true, "admin@thuvien.edu.vn", "NV_ADMIN", "Nguyễn Quản Trị (Admin)", "Admin@123", "0911223344", "Admin" },
                    { 2, true, "thuthu@thuvien.edu.vn", "NV_THUTHU", "Trần Thị Thủ Thư (Staff)", "Thuthu@123", "0988776655", "Librarian" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Active", "Author", "AvailableQuantity", "CategoryId", "CoverImage", "Description", "ISBN", "Price", "PublishYear", "Quantity", "Title" },
                values: new object[,]
                {
                    { 1, true, "Robert C. Martin", 8, 1, null, "Sách gối đầu giường của mọi lập trình viên chuyên nghiệp.", "9780132350884", 250000m, 2021, 10, "Clean Code: A Handbook of Agile Software Craftsmanship" },
                    { 2, true, "Andrew Hunt & David Thomas", 7, 1, null, "Hành trình từ lập trình viên nghiệp dư trở thành bậc thầy phát triển phần mềm.", "9780201616224", 280000m, 2022, 8, "The Pragmatic Programmer" },
                    { 3, true, "Dale Carnegie", 14, 4, null, "Nghệ thuật thu phục lòng người kinh điển.", "9786045882885", 95000m, 2023, 15, "Đắc Nhân Tâm" },
                    { 4, true, "Nguyễn Nhật Ánh", 11, 3, null, "Tác phẩm văn học lãng mạn đầy cảm xúc.", "9786042171120", 110000m, 2020, 12, "Mắt Biếc" },
                    { 5, true, "Yuval Noah Harari", 10, 5, null, "Khám phá toàn cảnh lịch sử tiến hóa và văn minh nhân loại.", "9780062316097", 185000m, 2022, 10, "Sapiens: Lược Sử Loài Người" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryId",
                table: "Books",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ISBN",
                table: "Books",
                column: "ISBN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
