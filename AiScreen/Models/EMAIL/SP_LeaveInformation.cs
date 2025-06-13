namespace MaxSystemWebSite.Models.EMAIL
{
    public class SP_LeaveInformation 
    {
        public SP_LeaveInformation()
        {
        }

        public SP_LeaveInformation(string email,decimal carry_Forward, decimal annual_Leave, decimal medical_Leave, decimal paternity_Leave, decimal maternity_Leave, decimal compassionate_Leave, decimal marriage_Leave, decimal convocation_Leave, decimal a_Carry_Forward, decimal a_Annual_Leave, decimal a_Medical_Leave, decimal a_Paternity_Leave, decimal a_Maternity_Leave, decimal a_Compassionate_Leave, decimal a_Marriage_Leave, decimal a_Convocation_Leave)
        {
            Email = email;
            Carry_Forward = carry_Forward;
            Annual_Leave = annual_Leave;
            Medical_Leave = medical_Leave;
            Paternity_Leave = paternity_Leave;
            Maternity_Leave = maternity_Leave;
            Compassionate_Leave = compassionate_Leave;
            Marriage_Leave = marriage_Leave;
            Convocation_Leave = convocation_Leave;
            A_Carry_Forward = a_Carry_Forward;
            A_Annual_Leave = a_Annual_Leave;
            A_Medical_Leave = a_Medical_Leave;
            A_Paternity_Leave = a_Paternity_Leave;
            A_Maternity_Leave = a_Maternity_Leave;
            A_Compassionate_Leave = a_Compassionate_Leave;
            A_Marriage_Leave = a_Marriage_Leave;
            A_Convocation_Leave = a_Convocation_Leave;
        }
        public string Email { get; set; }
        public decimal Carry_Forward { get; set; }
        public decimal Annual_Leave { get; set; }
        public decimal Medical_Leave { get; set; }
        public decimal Paternity_Leave { get; set; }
        public decimal Maternity_Leave { get; set; }    
        public decimal Compassionate_Leave { get; set; }
        public decimal Marriage_Leave { get; set; }
        public decimal Convocation_Leave { get; set; }

        public decimal A_Carry_Forward { get; set; }
        public decimal A_Annual_Leave { get; set; }
        public decimal A_Medical_Leave { get; set; }
        public decimal A_Paternity_Leave { get; set; }
        public decimal A_Maternity_Leave { get; set; }
        public decimal A_Compassionate_Leave { get; set; }
        public decimal A_Marriage_Leave { get; set; }
        public decimal A_Convocation_Leave { get; set; }
    }
}
