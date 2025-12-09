using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCOLLDB.Migrations
{
	/// <inheritdoc />
	public partial class Required : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Nif",
				table: "AspNetUsers",
				type: "nvarchar(9)",
				maxLength: 9,
				nullable: true,
				oldClrType: typeof(string),
				oldType: "nvarchar(9)",
				oldMaxLength: 9);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Nif",
				table: "AspNetUsers",
				type: "nvarchar(9)",
				maxLength: 9,
				nullable: false,
				defaultValue: "",
				oldClrType: typeof(string),
				oldType: "nvarchar(9)",
				oldMaxLength: 9,
				oldNullable: true);
		}
	}
}
