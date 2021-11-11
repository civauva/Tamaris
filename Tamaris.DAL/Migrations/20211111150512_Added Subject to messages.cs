using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tamaris.DAL.Migrations
{
    public partial class AddedSubjecttomessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "admin",
                table: "roles",
                keyColumn: "Id",
                keyValue: "984a3371-5ab4-41e4-adab-8920181bd032");

            migrationBuilder.DeleteData(
                schema: "admin",
                table: "roles",
                keyColumn: "Id",
                keyValue: "f9f0ae3e-8c50-44c2-8dc1-d23bd7a571d2");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                schema: "msg",
                table: "messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                schema: "admin",
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "is_admin", "Name", "NormalizedName" },
                values: new object[] { "8cdd3665-f2e7-4ff0-8c53-8c7fec3a9209", "3269cb52-ea59-4f5d-a6d3-1c31e8c1b493", true, "Administrators", "ADMINISTRATORS" });

            migrationBuilder.InsertData(
                schema: "admin",
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "is_admin", "Name", "NormalizedName" },
                values: new object[] { "d60ec769-7c02-41b7-bdbc-a5949c817a56", "d4f513f9-3309-4ca3-a53a-5d9541c1062f", false, "Standard users", "STANDARD USERS" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "admin",
                table: "roles",
                keyColumn: "Id",
                keyValue: "8cdd3665-f2e7-4ff0-8c53-8c7fec3a9209");

            migrationBuilder.DeleteData(
                schema: "admin",
                table: "roles",
                keyColumn: "Id",
                keyValue: "d60ec769-7c02-41b7-bdbc-a5949c817a56");

            migrationBuilder.DropColumn(
                name: "Subject",
                schema: "msg",
                table: "messages");

            migrationBuilder.InsertData(
                schema: "admin",
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "is_admin", "Name", "NormalizedName" },
                values: new object[] { "984a3371-5ab4-41e4-adab-8920181bd032", "2ae501bd-02d0-4a93-847e-d88b40e88ea5", true, "Administrators", "ADMINISTRATORS" });

            migrationBuilder.InsertData(
                schema: "admin",
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "is_admin", "Name", "NormalizedName" },
                values: new object[] { "f9f0ae3e-8c50-44c2-8dc1-d23bd7a571d2", "e3dd8c99-eeb5-4b4c-b569-bd1f9ce24814", false, "Standard users", "STANDARD USERS" });
        }
    }
}
