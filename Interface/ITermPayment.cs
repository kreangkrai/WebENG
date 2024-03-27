using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface ITermPayment
    {
        Term_PaymentModel GetByJob(string job);
        string Update(Term_PaymentModel term_Payment);
        string Insert(Term_PaymentModel term_Payment);
    }
}
