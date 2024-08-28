namespace FuelPriceWizard.BusinessLogic.Modules.Exceptions
{
    public class FuelPriceWizardLogicException : ApplicationException
    {
        public FuelPriceWizardLogicException(string? message) : base(message) { }

        public FuelPriceWizardLogicException(string? message, Exception innerException) : base(message, innerException) { }

    }
}
