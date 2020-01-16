using Dcidr.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dcidr.Excel
{
    public static class ExcelExporter
    {
        public static byte[] GenerateExcel(Decision decision)
        {
            if (!decision.ResultPrerequisitesMet)
            {
                throw new ArgumentException($"{nameof(decision)} does not have all prerequisites met");
            }

            using (var package = new ExcelPackage())
            {
                var criteriaRdvDictionary = BuildCriteriaComparisonSheet(package, decision);
                var optionRdvDictionary = new Dictionary<string, Dictionary<string, string>>();
                foreach(var criterion in decision.Criteria.Items)
                {
                    optionRdvDictionary.Add(criterion, BuildOptionComparisonSheet(package, decision, criterion));
                }
                BuildSymmarySheet(package, decision, criteriaRdvDictionary, optionRdvDictionary);
                return package.GetAsByteArray();
            }
        }

        private static void BuildSymmarySheet(ExcelPackage excelPackage, Decision decision, Dictionary<string,string> criteriaRdvDictionary, Dictionary<string, Dictionary<string,string>> optionRdvDictionary)
        {
            var sheet = excelPackage.Workbook.Worksheets.Add("Summary");

            var totalCriteria = decision.Criteria.Items.Count();
            var totalOptions = decision.Options.Items.Count();

            // criteria column labels
            for (int i = 0; i < totalCriteria; i++)
            {
                sheet.Cells[1, 2 + i].Value = decision.Criteria.Items.ElementAt(i);
            }

            // grand total column label
            sheet.Cells[1, totalCriteria + 2].Value = "Weighted Relative Decimal Value";

            for (int i = 0; i < totalOptions; i++)
            {
                // option row labels
                var option = decision.Options.Items.ElementAt(i);
                sheet.Cells[2 + i, 1].Value = option;
                for (int j = 0; j < totalCriteria; j++)
                {
                    var criterion = decision.Criteria.Items.ElementAt(j);
                    var criteriaRdvCellAddress = criteriaRdvDictionary[criterion];
                    var optionRdvCellAddress = optionRdvDictionary[criterion][option];
                    sheet.Cells[2 + i, 2 + j].Formula = $"={criteriaRdvCellAddress}*{optionRdvCellAddress}";
                }
                
                // grand total column
                var grandTotalRange = new ExcelAddress(2 + i, 2,
                    2 + i, 1 + totalCriteria);
                var grandTotalCellAddress = new ExcelCellAddress(2 + i, 2 + totalCriteria);
                sheet.Cells[grandTotalCellAddress.Address].Formula = $"=SUM({grandTotalRange})";
                sheet.Cells[grandTotalCellAddress.Address].Style.Font.Bold = true;
            }

            //this throws a PlatformNotSupportedException becuase of a call to GDI
            //sheet.Cells.AutoFitColumns();

            excelPackage.Workbook.Worksheets.MoveToStart("Summary");
        }

        /// <summary>
        /// Adds an option comparison sheet (for a criterion) to the package
        /// </summary>
        /// <param name="excelPackage"></param>
        /// <param name="decision"></param>
        /// <param name="criterion"></param>
        /// <returns>dictionary or RDV cell addresses one for each option</returns>
        private static Dictionary<string,string> BuildOptionComparisonSheet(ExcelPackage excelPackage, Decision decision, string criterion)
        {
            var optionRdvCellAddresses = new Dictionary<string, string>();

            var sheet = excelPackage.Workbook.Worksheets.Add(criterion);
            var totalOptions = decision.Options.Items.Count();
            sheet.Cells[1, 2 + totalOptions].Value = "Total";
            sheet.Cells[1, 3 + totalOptions].Value = "Relative Decimal Value";

            // grand total formula
            var grandTotalRange = new ExcelAddress(2, totalOptions + 2,
                totalOptions + 1, totalOptions + 2);
            var grandTotalCellAddress = new ExcelCellAddress(totalOptions + 2, totalOptions + 2);
            sheet.Cells[grandTotalCellAddress.Address].Formula = $"SUM({grandTotalRange})";

            for (int i = 0; i < totalOptions; i++)
            {
                var option = decision.Options.Items.ElementAt(i);

                // column labels
                sheet.Cells[1, 2 + i].Value = option;

                // row labels
                sheet.Cells[2 + i, 1].Value = option;

                // total column formula
                var totalRange = new ExcelAddress(2 + i, 2, 2 + i, 1 + totalOptions);
                var totalCellAddress = new ExcelCellAddress(2 + i, 2 + totalOptions);
                sheet.Cells[totalCellAddress.Address].Formula = $"SUM({totalRange})";

                // RDV column formula
                var optionRdvCellAddress = new ExcelCellAddress(2 + i, 3 + totalOptions);
                optionRdvCellAddresses.Add(option, $"{criterion}!{optionRdvCellAddress.Address}");
                sheet.Cells[optionRdvCellAddress.Address].Formula = $"{totalCellAddress.Address}/{grandTotalCellAddress.Address}";
            }

            // weights and inverses
            for (int i = 0; i < totalOptions; i++)
            {
                for (int j = 0; j < totalOptions; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    var optionOne = decision.Options.Items.ElementAt(i);
                    var optionTwo = decision.Options.Items.ElementAt(j);

                    var optionComparison = decision.OptionComparisons.FirstOrDefault(cc => cc.OptionOne == optionOne 
                        && cc.OptionTwo == optionTwo
                        && cc.Criterion == criterion);

                    if (optionComparison != null)
                    {
                        sheet.Cells[i + 2, j + 2].Value = optionComparison.Weight.Value.ToRatio();
                        sheet.Cells[j + 2, i + 2].Value = 1 / optionComparison.Weight.Value.ToRatio();
                    }
                }
            }
            return optionRdvCellAddresses;
        }

        /// <summary>
        /// Adds the criteria comparison sheet
        /// </summary>
        /// <param name="excelPackage"></param>
        /// <param name="decision"></param>
        /// <returns>Dictinary of cell addresses of criteria RDV values</returns>
        private static Dictionary<string, string> BuildCriteriaComparisonSheet(ExcelPackage excelPackage, Decision decision)
        {
            const string sheetTitle = "CriteriaComparisons";
            var criteriaRdvCellAddresses = new Dictionary<string, string>();

            var sheet = excelPackage.Workbook.Worksheets.Add(sheetTitle);
            var totalCriteria = decision.Criteria.Items.Count();
            sheet.Cells[1, 2 + totalCriteria].Value = "Total";
            sheet.Cells[1, 3 + totalCriteria].Value = "Relative Decimal Value";

            // grand total formula
            var grandTotalRange = new ExcelAddress(2, totalCriteria + 2,
                totalCriteria + 1, totalCriteria + 2);
            var grandTotalCellAddress = new ExcelCellAddress(totalCriteria + 2, totalCriteria + 2);
            sheet.Cells[grandTotalCellAddress.Address].Formula = $"SUM({grandTotalRange})";
            
            for (int i = 0; i < totalCriteria; i++)
            {
                var crit = decision.Criteria.Items.ElementAt(i);
                
                // column labels
                sheet.Cells[1, 2 + i].Value = crit;
                
                // row labels
                sheet.Cells[2 + i, 1].Value = crit;

                // total column formula
                var totalRange = new ExcelAddress(2 + i, 2,  2 + i, 1 + totalCriteria);
                var totalCellAddress = new ExcelCellAddress(2 + i, 2 + totalCriteria);
                sheet.Cells[totalCellAddress.Address].Formula = $"SUM({totalRange})";

                // RDV column formula
                var criteriaRdvAddress = new ExcelCellAddress(2 + i, 3 + totalCriteria);
                criteriaRdvCellAddresses.Add(crit, $"{sheetTitle}!{criteriaRdvAddress.Address}");
                sheet.Cells[criteriaRdvAddress.Address].Formula = $"{totalCellAddress.Address}/{grandTotalCellAddress.Address}";
            }

            // weights and inverses
            for (int i = 0; i < totalCriteria; i++)
            {
                for (int j = 0; j < totalCriteria; j++)
                {
                    if(i == j)
                    {
                        continue;
                    }
                    var critOne = decision.Criteria.Items.ElementAt(i);
                    var critTwo = decision.Criteria.Items.ElementAt(j);

                    var critComp = decision.CriteriaComparisons.FirstOrDefault(cc => cc.CriterionOne == critOne && cc.CriterionTwo == critTwo);
                    if (critComp != null)
                    {
                        sheet.Cells[i + 2, j + 2].Value = critComp.Weight.Value.ToRatio();
                        sheet.Cells[j + 2, i + 2].Value = 1 / critComp.Weight.Value.ToRatio();
                    }
                }
            }
            return criteriaRdvCellAddresses;
        }
    }
}
