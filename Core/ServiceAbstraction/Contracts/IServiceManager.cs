namespace Services.Abstraction.Contracts
{
    public interface IServiceManager
    {
        public IPatientService PatientService { get;  }
    }
}
