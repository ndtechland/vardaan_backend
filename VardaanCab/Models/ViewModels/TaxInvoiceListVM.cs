using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VardaanCab.Models.Domain;
using VardaanCab.Models.DTO;

namespace VardaanCab.Models.ViewModels
{
    public class TaxInvoiceFileListVM
    {
        public int Page { get; set; }
        public int NumberOfPages { get; set; }
        public string Term { get; set; }
        public List<TaxInvoiceFileDetails> TaxInvoiceFileLists{ get; set; }
      
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
        public int SrNo { get; set; }
        public StateWiseGSTIN StateGSTNBankdetails { get; set; }
    }
    public class TaxInvoiceFileDetails
    {
        public int Id { get; set; }
        public string InvoiceFile { get; set; }
        public string CreatedBy { get; set; }
        public string TaxInviceNo { get; set; }
        public DateTime TaxInvoiceDate { get; set; }
        public string PONo { get; set; }
        public DateTime PODate { get; set; }
        public bool IsCancel { get; set; }
        public string Remark { get; set; }
        public int Company_Id { get; set; }
        public int StateGstin_Id { get;set;}
        public string CompanyName { get; set; }
        public string City { get; set; }
        public double Amount { get; set; }        
        public double CGST { get; set; }
        public double CGST_per { get; set; }
        public double SGST { get; set; }
        public double SGST_per { get; set; }
        public double IGST { get; set; }
        public double IGST_per { get; set; }
        public double GrandTotal { get; set; }
        public string logDetails { get; set; }

    }

    public class TaxInvoiceListExcel
    {
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string TaxInviceNo { get; set; }
        public DateTime TaxInvoiceDate { get; set; }
        public string PONo { get; set; }
        public DateTime PODate { get; set; }
        public double Amount { get; set; }
        public double CGST { get; set; }
        public double CGST_per { get; set; }
        public double SGST { get; set; }
        public double SGST_per { get; set; }
        public double IGST { get; set; }
        public double IGST_per { get; set; }
        public double GrandTotal { get; set; }
    }
}