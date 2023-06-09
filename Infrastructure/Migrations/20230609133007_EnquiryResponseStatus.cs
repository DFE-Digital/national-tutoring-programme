﻿using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class EnquiryResponseStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnquirerRejectedReason",
                table: "EnquiryResponses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnquiryResponseStatusId",
                table: "EnquiryResponses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EnquiryResponseStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    OrderBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnquiryResponseStatus", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "EnquiryResponseStatus",
                columns: new[] { "Id", "Description", "OrderBy", "Status" },
                values: new object[,]
                {
                    { 1, "The enquirer has indicated that they are interested in the tuition partner response", 1, "Interested" },
                    { 2, "The enquirer has opened the tuition partner response, but has not confirmed if they are interested or not", 2, "Undecided" },
                    { 3, "The enquirer has not yet viewed the tuition partner response", 3, "Unread" },
                    { 4, "Status that is used for enquries that are historical and we don't have the latest status for", 4, "Not Set" },
                    { 5, "The enquirer has indicated that they are not interested in the tuition partner response", 5, "Rejected" }
                });

            //MANUAL CHANGES - START
            migrationBuilder.Sql("UPDATE \"EnquiryResponses\" SET \"EnquiryResponseStatusId\" = 4;", true);
            //MANUAL CHANGES - END

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryResponses_EnquiryResponseStatusId",
                table: "EnquiryResponses",
                column: "EnquiryResponseStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EnquiryResponseStatus_Status",
                table: "EnquiryResponseStatus",
                column: "Status",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EnquiryResponses_EnquiryResponseStatus_EnquiryResponseStatu~",
                table: "EnquiryResponses",
                column: "EnquiryResponseStatusId",
                principalTable: "EnquiryResponseStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnquiryResponses_EnquiryResponseStatus_EnquiryResponseStatu~",
                table: "EnquiryResponses");

            migrationBuilder.DropTable(
                name: "EnquiryResponseStatus");

            migrationBuilder.DropIndex(
                name: "IX_EnquiryResponses_EnquiryResponseStatusId",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "EnquirerRejectedReason",
                table: "EnquiryResponses");

            migrationBuilder.DropColumn(
                name: "EnquiryResponseStatusId",
                table: "EnquiryResponses");
        }
    }
}
