using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class CreateLoanProductDefaultCommand : IRequest<ServiceResponse<bool>>
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }

        public List<CreateLoanProductDefaultCommand> GetProductList()
        {
            return new List<CreateLoanProductDefaultCommand>
        {
            new CreateLoanProductDefaultCommand{ ProductId = "008277933871747", ProductName = "Loan categorie 3" },
            new CreateLoanProductDefaultCommand{ ProductId = "009453098559990", ProductName = "Technology Loans" },
            new CreateLoanProductDefaultCommand{ ProductId = "010902372901033", ProductName = "SSF-Education Loan ES" },
            new CreateLoanProductDefaultCommand{ ProductId = "011873937679142", ProductName = "ML-Agricultural Loan ES" },
            new CreateLoanProductDefaultCommand{ ProductId = "013724589753063", ProductName = "ML-Residential Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "018066463233754", ProductName = "ML- Health Loan CS" },
            new CreateLoanProductDefaultCommand{ ProductId = "028257201013413", ProductName = "ML-Commercial Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "033992317458255", ProductName = "CONTRACT" },
            new CreateLoanProductDefaultCommand{ ProductId = "036182162686844", ProductName = "ML- Health Loan Public" },
            new CreateLoanProductDefaultCommand{ ProductId = "052887523339201", ProductName = "ML-Health Loan ES" },
            new CreateLoanProductDefaultCommand{ ProductId = "095423906970742", ProductName = "SSF-Education Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "115644588843085", ProductName = "SSF Commercial Real Estate Loan C" },
            new CreateLoanProductDefaultCommand{ ProductId = "123967944893262", ProductName = "SSF-Salary Advance E" },
            new CreateLoanProductDefaultCommand{ ProductId = "129267623658487", ProductName = "ML-Residential Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "131107300919470", ProductName = "SSF-Agricultural Loan E" },
            new CreateLoanProductDefaultCommand{ ProductId = "144273647448124", ProductName = "SSF-Education Loan E" },
            new CreateLoanProductDefaultCommand{ ProductId = "176765310204055", ProductName = "ML-Salary Advance I" },
            new CreateLoanProductDefaultCommand{ ProductId = "182361221767404", ProductName = "Agricultural Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "183303737900592", ProductName = "ML- Consumption Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "195944467163251", ProductName = "ML-Residential Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "216302239689276", ProductName = "ML-Education Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "228348482566894", ProductName = "ML-Micro Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "234291563270193", ProductName = "ML-Residential Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "235377469201366", ProductName = "SSF-Consumption Loan ES" },
            new CreateLoanProductDefaultCommand{ ProductId = "235437614025989", ProductName = "LOAN RESCHEDULING" },
            new CreateLoanProductDefaultCommand{ ProductId = "240992305303372", ProductName = "SSF Commercial Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "256269341281171", ProductName = "SSF-Consumption Loan I" },
            new CreateLoanProductDefaultCommand{ ProductId = "260705705957403", ProductName = "SSF-Business Loan C" },
            new CreateLoanProductDefaultCommand{ ProductId = "270815502522227", ProductName = "ML-Social Project Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "281766310144456", ProductName = "SSF-Education Loan I" },
            new CreateLoanProductDefaultCommand{ ProductId = "286856340212910", ProductName = "SSF Commercial Real Estate Loan G" },
            new CreateLoanProductDefaultCommand{ ProductId = "287529554567484", ProductName = "OTHERS" },
            new CreateLoanProductDefaultCommand{ ProductId = "301432559799083", ProductName = "ML-Education Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "312538664270452", ProductName = "ML Micro Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "325151969211758", ProductName = "ML- Consumption Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "333133642990941", ProductName = "ML-Agricultural Loan I" },
            new CreateLoanProductDefaultCommand{ ProductId = "336410847423249", ProductName = "SSF- Health Loans L" },
            new CreateLoanProductDefaultCommand{ ProductId = "337041938977778", ProductName = "Livestock Loans" },
            new CreateLoanProductDefaultCommand{ ProductId = "339561039774411", ProductName = "ML- Agricultural Loan M" },
            new CreateLoanProductDefaultCommand{ ProductId = "344176817222200", ProductName = "SSF Business loans" },
            new CreateLoanProductDefaultCommand{ ProductId = "349260197680596", ProductName = "SSF Residential Real Estate Loan G" },
            new CreateLoanProductDefaultCommand{ ProductId = "350417263601638", ProductName = "SSF-Salary Advance I" },
            new CreateLoanProductDefaultCommand{ ProductId = "351820219655945", ProductName = "SSF-Education Loan G" },
            new CreateLoanProductDefaultCommand{ ProductId = "361297370983865", ProductName = "ML-Salary Advance ES" },
            new CreateLoanProductDefaultCommand{ ProductId = "366736953582866", ProductName = "ML-Commercial Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "369429327843104", ProductName = "ML- Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "384770173704102", ProductName = "SSF Consumption Loan Private" },
            new CreateLoanProductDefaultCommand{ ProductId = "385213642657608", ProductName = "ML-Micro Business Loan M" },
            new CreateLoanProductDefaultCommand{ ProductId = "390284231369509", ProductName = "Micro Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "390864281585292", ProductName = "BUILDING" },
            new CreateLoanProductDefaultCommand{ ProductId = "392307941594005", ProductName = "ML-Education Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "409502033863734", ProductName = "ML-Micro Business Loan P" },
            new CreateLoanProductDefaultCommand{ ProductId = "418169817029448", ProductName = "SSF Agricultural Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "432057118671760", ProductName = "SSF Residential Real Estate Loan L" },
            new CreateLoanProductDefaultCommand{ ProductId = "439072179178835", ProductName = "SSF-Consumption Loan L" },
            new CreateLoanProductDefaultCommand{ ProductId = "449038405110665", ProductName = "SSF Commercial Real Estate Loan E" },
            new CreateLoanProductDefaultCommand{ ProductId = "459554827321548", ProductName = "Education Loans" },
            new CreateLoanProductDefaultCommand{ ProductId = "464074539035044", ProductName = "ML- Health Loan Private" },
            new CreateLoanProductDefaultCommand{ ProductId = "464724876816005", ProductName = "ML- Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "472457509476351", ProductName = "ML-Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "490239117580354", ProductName = "SSF-Agricultural Loan I" },
            new CreateLoanProductDefaultCommand{ ProductId = "493895924609218", ProductName = "SSF-Agricultural Loan C" },
            new CreateLoanProductDefaultCommand{ ProductId = "493952841905043", ProductName = "SSF Residential Real Estate Loan PS" },
            new CreateLoanProductDefaultCommand{ ProductId = "500925424418713", ProductName = "HEALTH" },
            new CreateLoanProductDefaultCommand{ ProductId = "505279078131888", ProductName = "SSF Commercial Real Estate Loan I" },
            new CreateLoanProductDefaultCommand{ ProductId = "507016845395014", ProductName = "SSF-Micro Business Loan E" },
            new CreateLoanProductDefaultCommand{ ProductId = "507208935254624", ProductName = "ML-Commercial Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "509592666375018", ProductName = "Real Estate Loans SSF" },
            new CreateLoanProductDefaultCommand{ ProductId = "526038078143114", ProductName = "SSF-Education Loan L" },
            new CreateLoanProductDefaultCommand{ ProductId = "527234211135542", ProductName = "SSF-Business Loan L" },
            new CreateLoanProductDefaultCommand{ ProductId = "528434239499275", ProductName = "SSF-Micro Business Loan ES" },
            new CreateLoanProductDefaultCommand{ ProductId = "539412728831362", ProductName = "Loan Product" },
            new CreateLoanProductDefaultCommand{ ProductId = "551526351093426", ProductName = "ML-Residential Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "551566275006262", ProductName = "ML-Residential Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "551920735686564", ProductName = "Housing Loans" },
            new CreateLoanProductDefaultCommand{ ProductId = "561746931894346", ProductName = "SSF-Social Project Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "571186335777284", ProductName = "ML- Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "576996807535608", ProductName = "SSF-Business Loan I" },
            new CreateLoanProductDefaultCommand{ ProductId = "592893328108952", ProductName = "SSF -Micro Business Loan I" },
            new CreateLoanProductDefaultCommand{ ProductId = "594148435395794", ProductName = "SSF Commercial Real Estate Loan ES" },
            new CreateLoanProductDefaultCommand{ ProductId = "595008670276729", ProductName = "ML- Consumption Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "601674013040454", ProductName = "ML- Health Loan I" },
            new CreateLoanProductDefaultCommand{ ProductId = "605301049140999", ProductName = "SSF-Agricultural Loan L" },
            new CreateLoanProductDefaultCommand{ ProductId = "612261529585246", ProductName = "Business Loan (Special Savings Facility)" },
            new CreateLoanProductDefaultCommand{ ProductId = "612752681257545", ProductName = "SSF-Residential Real Estate Loan ES" },
            new CreateLoanProductDefaultCommand{ ProductId = "616627358724874", ProductName = "SSF-Agricultural Loan PS" },
            new CreateLoanProductDefaultCommand{ ProductId = "619571630836059", ProductName = "SSF-Special Salary Real Estate Loan I" },
            new CreateLoanProductDefaultCommand{ ProductId = "624526926067944", ProductName = "SSF Commercial Real Estate Loan L" },
            new CreateLoanProductDefaultCommand{ ProductId = "631182443809770", ProductName = "ML- Agricultural Loan Private" },
            new CreateLoanProductDefaultCommand{ ProductId = "635657351419064", ProductName = "ML-Commercial Real Estate Loan E" },
            new CreateLoanProductDefaultCommand{ ProductId = "640361257747063", ProductName = "SSF-Consumption Loan E" },
            new CreateLoanProductDefaultCommand{ ProductId = "646757030230130", ProductName = "Micro Business Loan L" },
            new CreateLoanProductDefaultCommand{ ProductId = "666064536604703", ProductName = "ML-Commercial Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "671400352671099", ProductName = "Individual Business Loans" },
            new CreateLoanProductDefaultCommand{ ProductId = "681542354859741", ProductName = "Micro Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "691252254949085", ProductName = "ML- Health Loan E" },
            new CreateLoanProductDefaultCommand{ ProductId = "699430864141030", ProductName = "Microenterprise Loans" },
            new CreateLoanProductDefaultCommand{ ProductId = "703396929510449", ProductName = "ML-Residential Real Estate L" },
            new CreateLoanProductDefaultCommand{ ProductId = "706502278481136", ProductName = "SSF-Health Loan ES" },
            new CreateLoanProductDefaultCommand{ ProductId = "713381882388352", ProductName = "SSF Residential Real Estate Loan I" },
            new CreateLoanProductDefaultCommand{ ProductId = "726899791436801", ProductName = "SSF-Health Loan C" },
            new CreateLoanProductDefaultCommand{ ProductId = "732330093748526", ProductName = "SSF-Health Loan P" },
            new CreateLoanProductDefaultCommand{ ProductId = "740840146976270", ProductName = "ML-Education Loan c" },
            new CreateLoanProductDefaultCommand{ ProductId = "743730890825029", ProductName = "ML- Consumption Loan Public" },
            new CreateLoanProductDefaultCommand{ ProductId = "750588590805260", ProductName = "Agricultural Loans" },
            new CreateLoanProductDefaultCommand{ ProductId = "761189955442418", ProductName = "ML-Salary Advance E" },
            new CreateLoanProductDefaultCommand{ ProductId = "765532832473758", ProductName = "COMMERCE" },
            new CreateLoanProductDefaultCommand{ ProductId = "767643510711588", ProductName = "SSF-Business Loan G" },
            new CreateLoanProductDefaultCommand{ ProductId = "784847556661901", ProductName = "ML-Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "799398023737661", ProductName = "SSF-Education Loan C" },
            new CreateLoanProductDefaultCommand{ ProductId = "810775748732214", ProductName = "SSF-Business Loan E" },
            new CreateLoanProductDefaultCommand{ ProductId = "811549627604972", ProductName = "EDUCATION" },
            new CreateLoanProductDefaultCommand{ ProductId = "812537810098406", ProductName = "ML- Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "813587966756777", ProductName = "Group Loans" },
            new CreateLoanProductDefaultCommand{ ProductId = "814789549407993", ProductName = "ML- Consumption Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "823036881917845", ProductName = "SSF-Consumption Loan C" },
            new CreateLoanProductDefaultCommand{ ProductId = "836662411099890", ProductName = "ML- Agricultural Loan E" },
            new CreateLoanProductDefaultCommand{ ProductId = "843740243913604", ProductName = "ML-Education Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "846481664896691", ProductName = "Micro Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "865847643450339", ProductName = "ML- Education Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "877844607379080", ProductName = "ML-Commercial Real Estate Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "885396244140877", ProductName = "ML- Health Loan M" },
            new CreateLoanProductDefaultCommand{ ProductId = "886406453328720", ProductName = "ML-Special Salary Real Estate Loan I" },
            new CreateLoanProductDefaultCommand{ ProductId = "906879767272114", ProductName = "ML- Business Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "908163540098201", ProductName = "ML-Commercial Real Estate Loan" },
              new CreateLoanProductDefaultCommand{ ProductId = "918697293785053", ProductName = "SSF-Agricultural Loan ES" },
            new CreateLoanProductDefaultCommand{ ProductId = "932105886682485", ProductName = "ML-Education Loan" },
            new CreateLoanProductDefaultCommand{ ProductId = "934818385387921", ProductName = "SSF-Business Loan ES" },
            new CreateLoanProductDefaultCommand{ ProductId = "935714358546540", ProductName = "ML- Consumption Loan Private" },
            new CreateLoanProductDefaultCommand{ ProductId = "936673022784439", ProductName = "SSF Residential Real Estate Loan E" },
            new CreateLoanProductDefaultCommand{ ProductId = "939592137467363", ProductName = "MEDICAL" },
            new CreateLoanProductDefaultCommand{ ProductId = "942209034424487", ProductName = "CONSUMPTION" },
            new CreateLoanProductDefaultCommand{ ProductId = "946041009792295", ProductName = "EQUIPEMENT" },
            new CreateLoanProductDefaultCommand{ ProductId = "952716366312401", ProductName = "SSF-Health Loan E" },
            new CreateLoanProductDefaultCommand{ ProductId = "953777001543259", ProductName = "SSF-Consumption Loan G" }
          };


        }
    }
    public class Rubrique
    {
        public string Id { get; set; }
        public string RubriqueName { get; set; }

        public static List<Rubrique> GetAllRubrique()
        {
            return new List<Rubrique>
        {
             new Rubrique{ Id="CMP7881724497",RubriqueName="Loan_Principal_Account"},
           new Rubrique{ Id="CMP4137219525",RubriqueName="Loan_VAT_Account"},
           new Rubrique{ Id="CMP5345478821",RubriqueName="Loan_Interest_Recieved_Account"},
           new Rubrique{ Id="CMP2489415618",RubriqueName="Loan_Penalty_Account"},
           new Rubrique{ Id="CMP2489415618",RubriqueName="Loan_WriteOff_Account"},
           new Rubrique{ Id="CMP4334955740",RubriqueName="Loan_Provisioning_Account_MoreThanOneYear"},
           new Rubrique{ Id="CMP4334955740",RubriqueName="Loan_Provisioning_Account_MoreThanTwoYear"},
           new Rubrique{ Id="CMP4334955740",RubriqueName="Loan_Provisioning_Account_MoreThanThreeYear"},
           new Rubrique{ Id="CMP4334955740",RubriqueName="Loan_Provisioning_Account_MoreThanFourYear"},
            new Rubrique{ Id="CMP9615641930",RubriqueName="Loan_Transit_Account"},
                    new Rubrique{ Id="CMP2489415618",RubriqueName="Loan_Product"}
        };
        }


        public static List<ProductAccountBookDetail> ConvertRubriqueToProductAccountBookDetail(List<Rubrique> rubriques)
        {
            if (rubriques == null || !rubriques.Any())
                return new List<ProductAccountBookDetail>();

            return rubriques.Select(r => new ProductAccountBookDetail
            {
                Name = r.RubriqueName,
                ChartOfAccountId = r.Id
            }).ToList();
        }
    }
}
