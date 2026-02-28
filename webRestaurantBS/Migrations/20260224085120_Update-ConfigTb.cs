using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webRestaurantBS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConfigTb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbMenus_TbMenus_ParentId",
                table: "TbMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_TbPermissionRoles_TbMenus_MenuId",
                table: "TbPermissionRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_TbPermissionRoles_TbRoles_RoleId",
                table: "TbPermissionRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_TbUsers_TbRoles_RoleId",
                table: "TbUsers");

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "TbRoles",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RoleCaption",
                table: "TbRoles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NameInSystem",
                table: "TbMenus",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IconName",
                table: "TbMenus",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FaDate",
                table: "TbMenus",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "TbMenus",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Caption",
                table: "TbMenus",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_TbMenus_TbMenus_ParentId",
                table: "TbMenus",
                column: "ParentId",
                principalTable: "TbMenus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TbPermissionRoles_TbMenus_MenuId",
                table: "TbPermissionRoles",
                column: "MenuId",
                principalTable: "TbMenus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TbPermissionRoles_TbRoles_RoleId",
                table: "TbPermissionRoles",
                column: "RoleId",
                principalTable: "TbRoles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TbUsers_TbRoles_RoleId",
                table: "TbUsers",
                column: "RoleId",
                principalTable: "TbRoles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbMenus_TbMenus_ParentId",
                table: "TbMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_TbPermissionRoles_TbMenus_MenuId",
                table: "TbPermissionRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_TbPermissionRoles_TbRoles_RoleId",
                table: "TbPermissionRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_TbUsers_TbRoles_RoleId",
                table: "TbUsers");

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "TbRoles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "RoleCaption",
                table: "TbRoles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "NameInSystem",
                table: "TbMenus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "IconName",
                table: "TbMenus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "FaDate",
                table: "TbMenus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "TbMenus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Caption",
                table: "TbMenus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddForeignKey(
                name: "FK_TbMenus_TbMenus_ParentId",
                table: "TbMenus",
                column: "ParentId",
                principalTable: "TbMenus",
                principalColumn: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_TbPermissionRoles_TbMenus_MenuId",
                table: "TbPermissionRoles",
                column: "MenuId",
                principalTable: "TbMenus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TbPermissionRoles_TbRoles_RoleId",
                table: "TbPermissionRoles",
                column: "RoleId",
                principalTable: "TbRoles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TbUsers_TbRoles_RoleId",
                table: "TbUsers",
                column: "RoleId",
                principalTable: "TbRoles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
