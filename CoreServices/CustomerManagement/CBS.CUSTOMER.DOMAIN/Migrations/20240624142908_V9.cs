using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBS.CUSTOMER.DOMAIN.Migrations
{
    /// <inheritdoc />
    public partial class V9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CNI",
                table: "UploadedCustomerWithErrors",
                newName: "Cni");

            migrationBuilder.RenameColumn(
                name: "MembersSecondName",
                table: "UploadedCustomerWithErrors",
                newName: "Town");

            migrationBuilder.RenameColumn(
                name: "MembersFirstName",
                table: "UploadedCustomerWithErrors",
                newName: "Quater");

            migrationBuilder.RenameColumn(
                name: "LieuCNI",
                table: "UploadedCustomerWithErrors",
                newName: "PlaceOfBirth");

            migrationBuilder.RenameColumn(
                name: "DateCNI",
                table: "UploadedCustomerWithErrors",
                newName: "MemberSurName");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "UploadedCustomerWithErrors",
                newName: "MemberName");

            migrationBuilder.RenameColumn(
                name: "AdhesionDate",
                table: "UploadedCustomerWithErrors",
                newName: "DateOfBirth");

            migrationBuilder.AddColumn<string>(
                name: "CniDeliveranceDate",
                table: "UploadedCustomerWithErrors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CniLocation",
                table: "UploadedCustomerWithErrors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreationDate",
                table: "UploadedCustomerWithErrors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CniDeliveranceDate",
                table: "UploadedCustomerWithErrors");

            migrationBuilder.DropColumn(
                name: "CniLocation",
                table: "UploadedCustomerWithErrors");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "UploadedCustomerWithErrors");

            migrationBuilder.RenameColumn(
                name: "Cni",
                table: "UploadedCustomerWithErrors",
                newName: "CNI");

            migrationBuilder.RenameColumn(
                name: "Town",
                table: "UploadedCustomerWithErrors",
                newName: "MembersSecondName");

            migrationBuilder.RenameColumn(
                name: "Quater",
                table: "UploadedCustomerWithErrors",
                newName: "MembersFirstName");

            migrationBuilder.RenameColumn(
                name: "PlaceOfBirth",
                table: "UploadedCustomerWithErrors",
                newName: "LieuCNI");

            migrationBuilder.RenameColumn(
                name: "MemberSurName",
                table: "UploadedCustomerWithErrors",
                newName: "DateCNI");

            migrationBuilder.RenameColumn(
                name: "MemberName",
                table: "UploadedCustomerWithErrors",
                newName: "Balance");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "UploadedCustomerWithErrors",
                newName: "AdhesionDate");
        }
    }
}
