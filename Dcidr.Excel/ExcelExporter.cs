using Dcidr.Model;
using OfficeOpenXml;
using System;
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
                ExcelWorksheet criteriaComparisons = package.Workbook.Worksheets.Add("CriteriaComparisons");
                BuildCriteriaComparisonSheet(criteriaComparisons, decision);
                return package.GetAsByteArray();
            }
        }

        private static void BuildCriteriaComparisonSheet(ExcelWorksheet sheet, Decision decision)
        {
            var totalCriteria = decision.Criteria.Items.Count();
            sheet.Cells[1, 2 + totalCriteria].Value = "Total";
            sheet.Cells[1, 3 + totalCriteria].Value = "Relative Decimal Value";

            // grand total formuma
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
                sheet.Cells[2 + i, 3 + totalCriteria].Formula = $"{totalCellAddress.Address}/{grandTotalCellAddress.Address}";
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
        }
    }
}
