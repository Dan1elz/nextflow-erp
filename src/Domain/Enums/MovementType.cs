namespace dotnet_api_erp.src.Domain.Enums
{
    public enum MovementType : byte
    {
        Entry = 1,          //Entrada
        Exit = 2,           //Remoção
        Adjustment = 3,     //Ajuste
        Sales = 4,          //Vendas
        Return = 5,         //Retorno
        Transfer = 6        //Transferência
    }
}