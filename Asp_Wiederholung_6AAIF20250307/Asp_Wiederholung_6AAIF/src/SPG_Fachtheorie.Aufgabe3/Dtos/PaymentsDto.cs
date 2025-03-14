using SPG_Fachtheorie.Aufgabe1.Model;
namespace SPG_Fachtheorie.Aufgabe3.Dtos
{
    public record PaymentsDto(
        int Id, string EmployeeFirstName, string EmployeeLastName,
        int CashDeskNumber, string PaymentType, decimal TotalAmount);

    public record PaymentItemDto(
            string ArticleName,
            int Amount,
            decimal Price);

    public record PaymentDetailDto(
            int Id,
            string EmployeeFirstName,
            string EmployeeLastName,
            int CashDeskNumber,
            string PaymentType,
            List<PaymentItemDto> PaymentItems);
}
