// ======================================================
// Program for calculating McIlvaine Buffer capacity
// Author: Daniel A.G.
// 2025 | Leibniz Institute for Polymer Research Dresden
// ======================================================


using System.Diagnostics;
using System.Globalization;

class Program
{    
    public static void Main(string[] args)
    {
        GetTau(args);
        //TauGraph();
    }

    public static void GetTau(string[] args)
    {
        double pH;
        double citr_conc;
        double phos_conc;
        Console.WriteLine();
        if (args.Length != 3) { Console.WriteLine($"Wrong usage! Syntax: \"{Process.GetCurrentProcess().ProcessName}.exe\" <pH> <citric acid concentration [mol/l]> <disodium phosphate concentration [mol/l]>"); return; }
        try
        {
            pH = Double.Parse(args[0], CultureInfo.InvariantCulture);
            citr_conc = Double.Parse(args[1], CultureInfo.InvariantCulture);
            phos_conc = Double.Parse(args[2], CultureInfo.InvariantCulture);
        }
        catch (Exception ex) { Console.WriteLine($"Invalid input format. Example:\n\"{Process.GetCurrentProcess().ProcessName}.exe\" 3 0.05 0.05"); return; }   // user proofing
        if (pH < 0 || pH > 14) { Console.WriteLine("pH must be between 0 - 14"); return; }                                                                      // user proofing
        if (citr_conc <= 0 || phos_conc <= 0) { Console.WriteLine("concentrations can't be negative"); return; };                                               // user proofing


        var bufferData = ChemCalc.calc_tau(pH, citr_conc, phos_conc);   // perform the calculation





        Console.WriteLine("================================= RESULT =================================");
        Console.WriteLine("Source:  https://doi.org/10.18540/jcecvl6iss3pp0387-0396");
        Console.WriteLine();
        Console.WriteLine("Input:");
        Console.WriteLine($"\tpH:                           {pH.ToString("F2", CultureInfo.InvariantCulture)}");
        Console.WriteLine($"\tCitric Acid conc.:            {citr_conc.ToString("F4", CultureInfo.InvariantCulture)} mol/l");
        Console.WriteLine($"\tDisodium Phosphate conc.:     {phos_conc.ToString("F4", CultureInfo.InvariantCulture)} mol/l");
        Console.WriteLine();
        Console.WriteLine("Effective Charge (Source:  https://www.sciencedirect.com/topics/engineering/effective-charge):");
        Console.WriteLine($"\tCitric Acid:                  {bufferData.qeff_citr.ToString("F2", CultureInfo.InvariantCulture)}");
        Console.WriteLine($"\tDisodium Phosphate:           {bufferData.qeff_phos.ToString("F2", CultureInfo.InvariantCulture)}");
        Console.WriteLine();
        Console.WriteLine($"Water contribution:             {bufferData.wat.ToString("F4", CultureInfo.InvariantCulture)} mol/l");
        Console.WriteLine($"Citric Acid contribution:       {bufferData.citr_contrib.ToString("F4", CultureInfo.InvariantCulture)} mol/l");
        Console.WriteLine($"Phosphoric Acid contribution:   {bufferData.phos_contrib.ToString("F4", CultureInfo.InvariantCulture)} mol/l");
        Console.WriteLine("--------------------------------------------------------------------------");
        Console.WriteLine($"Buffer Function:                {bufferData.tau.ToString("F4", CultureInfo.InvariantCulture)} mol/l");
        Console.WriteLine();
        Console.WriteLine($"Kolthoff's buffer capacity:     {ChemCalc.calc_KolthoffBufferCap(pH,citr_conc,phos_conc).ToString("F4", CultureInfo.InvariantCulture)} mol/l");
        Console.WriteLine("==========================================================================");
    }
    public static void TauGraph()                                           // Test function to obtain the Buffering Function as highlighted in: BUFFERING FUNCTION: A GENERAL APPROACH FOR BUFFER BEHAVIOR, A.F.OLIVEIRA, 2020, doi: 10.18540/jcecvl6iss3pp0387-0396
    {
        double[] pHArray = [1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0, 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7, 2.8, 2.9, 3.0, 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 3.9, 4, 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 4.7, 4.8, 4.9, 5, 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7, 5.8, 5.9, 6, 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7, 6.8, 6.9, 7, 7.1, 7.2, 7.3, 7.4, 7.5, 7.6, 7.7, 7.8, 7.9, 8,  8.1, 8.2,8.3, 8.4, 8.5, 8.6, 8.7, 8.8 ,8.9, 9 ,  9.1 ,9.2, 9.3, 9.4, 9.5, 9.6, 9.7, 9.8 ,9.9 ,10  ,10.1,    10.2 ,   10.3 ,   10.4  ,  10.5 ,   10.6  ,  10.7  ,  10.8  ,  10.9  ,  11  ,11.1   , 11.2    ,11.3    ,11.4   , 11.5   , 11.6    ,11.7   , 11.8  ,  11.9  ,  12];
        // yes I know this a terrible way to iterate, no I will not fix this :/

        double citr_conc = 0.050;                                        // concentration of citric acid in the McIlvaine Buffer: 50 mmol/l
        double phos_conc = 0.050;                                        // concentration of disodium phosphate in the McIlvaine Buffer: 50 mmol/l

        Console.WriteLine("pH;Water Contribution;Citric Acid Contribution;Disodium phosphate Contribution;Tau");
        
        NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
        foreach (double pH in pHArray)
        {
            ChemCalc.BufferData bufferData = ChemCalc.calc_tau(pH, citr_conc, phos_conc);
            Console.WriteLine($"{pH.ToString(nfi)};{bufferData.wat.ToString(nfi)};{bufferData.citr_contrib.ToString(nfi)};{bufferData.phos_contrib.ToString(nfi)};{bufferData.tau.ToString(nfi)}");
        }
    }



}


class ChemCalc
{
    public class BufferData
    {
        public double wat;
        public double qeff_citr;
        public double qeff_phos;
        public double citr_contrib;
        public double phos_contrib;
        public double tau;
    }

    static double pKw = 14;                                                         // just the pKw of water
    static List<double> citricAcidpKaValues = new() { 3.13, 4.76, 6.39, 14.4 };     // improve reference! --> Source: https://en.wikipedia.org/wiki/Citric_acid
    static List<double> phosphAcidpkAValues = new() { 2.12, 7.21, 12.32 };          // improve reference! --> Source: https://www.wyzant.com/resources/answers/519411/phosphoric_acid_h3po4_has_three_dissociable_protons_it_is_triprotic_thus_three_pka_values_2_12_7_21_and_12_32_what_volume_of_200_mm_phosphoric_acid_and_w

    public static BufferData calc_tau(double pH, double citr_conc, double phos_conc)
    {
        double tau = 0;

        BufferData data = new BufferData();

        data.wat = ChemCalc.calc_watercontribution(pH);
        data.qeff_citr = ChargeCalc.Method2.calc_qeff(pH, citricAcidpKaValues);
        data.qeff_phos = ChargeCalc.Method2.calc_qeff(pH, phosphAcidpkAValues);

        data.citr_contrib = data.qeff_citr * citr_conc;                             // Source: BUFFERING FUNCTION: A GENERAL APPROACH FOR BUFFER BEHAVIOR, A.F.OLIVEIRA, 2020, doi: 10.18540/jcecvl6iss3pp0387-0396
        data.phos_contrib = data.qeff_phos * phos_conc;                             // Source: BUFFERING FUNCTION: A GENERAL APPROACH FOR BUFFER BEHAVIOR, A.F.OLIVEIRA, 2020, doi: 10.18540/jcecvl6iss3pp0387-0396

        data.tau = data.wat + data.citr_contrib + data.phos_contrib;                // Source: BUFFERING FUNCTION: A GENERAL APPROACH FOR BUFFER BEHAVIOR, A.F.OLIVEIRA, 2020, doi: 10.18540/jcecvl6iss3pp0387-0396
        //Console.WriteLine($"{pH};{citr_contrib};{phos_contrib};{tau}");
        return data;
    }

    public static double calc_KolthoffBufferCap(double pH, double citr_conc, double phos_conc)
    {
        double pH1 = pH - 0.5;
        double pH2 = pH + 0.5;

        double tau_1 = calc_tau(pH1, citr_conc, phos_conc).tau;                     // Source: BUFFERING FUNCTION: A GENERAL APPROACH FOR BUFFER BEHAVIOR, A.F.OLIVEIRA, 2020, doi: 10.18540/jcecvl6iss3pp0387-0396
        double tau_2 = calc_tau(pH2, citr_conc, phos_conc).tau;                     // Source: BUFFERING FUNCTION: A GENERAL APPROACH FOR BUFFER BEHAVIOR, A.F.OLIVEIRA, 2020, doi: 10.18540/jcecvl6iss3pp0387-0396

        return -(tau_2-tau_1);                                                      // Source: BUFFERING FUNCTION: A GENERAL APPROACH FOR BUFFER BEHAVIOR, A.F.OLIVEIRA, 2020, doi: 10.18540/jcecvl6iss3pp0387-0396
    }

    private static double calc_watercontribution(double pH)
    {
        return Math.Pow(10, -pH) - Math.Pow(10, pH - pKw);                          // Source: BUFFERING FUNCTION: A GENERAL APPROACH FOR BUFFER BEHAVIOR, A.F.OLIVEIRA, 2020, doi: 10.18540/jcecvl6iss3pp0387-0396
    }
}



class ChargeCalc
{
    public class Method1                                                            // Source: BUFFERING FUNCTION: A GENERAL APPROACH FOR BUFFER BEHAVIOR, A.F.OLIVEIRA, 2020, doi: 10.18540/jcecvl6iss3pp0387-0396
    {
        private static void calc_a0()
        {
            
        }
    }
    public class Method2                                                            // Calculate the effective charge from pKa - Source: https://www.sciencedirect.com/topics/engineering/effective-charge
    {
        public static double calc_qeff(double pH, List<double> pKaList)         
        {
            return calc_qp(pH, pKaList) - calc_qn(pH, pKaList);
        }

        private static double calc_qn(double pH, List<double> pKaList)
        {
            double qn = 0;

            foreach (var pKa in pKaList)
            {
                qn += (Math.Pow(10, pH - pKa)) / (1 + Math.Pow(10, pH - pKa));      // Source: https://www.sciencedirect.com/topics/engineering/effective-charge
            }

            //Console.WriteLine($"qn: {qn}");
            return qn;
        }
        private static double calc_qp(double pH, List<double> pKaList)
        {
            double qp = 0;

            foreach (var pKa in pKaList)
            {
                qp += 1 / (1 + Math.Pow(10, pH - pKa));                         // Source: https://www.sciencedirect.com/topics/engineering/effective-charge
            }

            //Console.WriteLine($"qp: {qp}");
            return qp;
        }
    }


}
