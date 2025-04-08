using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface ISummaryJobInHand
    {
        List<SummaryENGJobInHandModel> GetsSummaryENGJobInHand(int year, string type);
        List<SummaryCISJobInHandModel> GetsSummaryCISJobInHand(int year, string type);
        List<SummaryAISJobInHandModel> GetsSummaryAISJobInHand(int year, string type);

        List<SummaryENGJobInHandModel> GetsProjectENGJobInHand(int year, string type);
        List<SummaryCISJobInHandModel> GetsProjectCISJobInHand(int year, string type);
        List<SummaryAISJobInHandModel> GetsProjectAISJobInHand(int year, string type);

        List<SummaryENGJobInHandModel> GetsAccENGJobInHand(int year, string type);
        List<SummaryCISJobInHandModel> GetsAccCISJobInHand(int year, string type);
        List<SummaryAISJobInHandModel> GetsAccAISJobInHand(int year, string type);

        List<SummaryENGJobInHandModel> GetsServiceENGJobInHand(int year, string type);
        List<SummaryCISJobInHandModel> GetsServiceCISJobInHand(int year, string type);
        List<SummaryAISJobInHandModel> GetsServiceAISJobInHand(int year, string type);

        List<JobENGInhandModel> GetsENGJobInhand(int year);
        List<JobCISInhandModel> GetsCISJobInhand(int year);
        List<JobAISInhandModel> GetsAISJobInhand(int year);

        List<JobENGInhandModel> GetsENGJobBackLog(int year);
        List<JobCISInhandModel> GetsCISJobBackLog(int year);
        List<JobAISInhandModel> GetsAISJobBackLog(int year);

        List<QuarterENGModel> GetsSummaryENGQuarter(int year);
        List<QuarterCISModel> GetsSummaryCISQuarter(int year);
        List<QuarterAISModel> GetsSummaryAISQuarter(int year);

        OrderInTakeENGModel GetOrderENGInTake(int year);
        OrderInTakeCISModel GetOrderCISInTake(int year);
        OrderInTakeAISModel GetOrderAISInTake(int year);
    }
}
