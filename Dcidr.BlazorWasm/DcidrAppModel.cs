using Dcidr.Model;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dcidr.BlazorWasm
{
    public class DcidrAppModel
    {
        public Decision Decision { get; set; } = new Decision();

        public void NewDecision()
        {
            Decision = new Decision();
        }

        public void EnforeStepPrerequisites(DcidrStep step, NavigationManager navigationManager)
        {
            switch (step)
            {
                case DcidrStep.Criteria:
                    if (!Decision.HasEnoughOptions)
                    {
                        navigationManager.NavigateTo("/options");
                    }
                    break;
                case DcidrStep.CompareOptions:
                    if (!Decision.HasEnoughCriteria)
                    {
                        navigationManager.NavigateTo("/criteria");
                    }
                    break;
                case DcidrStep.CompareCriteria:
                    if (!Decision.AllOptionComparisonsHaveWeights)
                    {
                        navigationManager.NavigateTo("/compare-options");
                    }
                    break;
                case DcidrStep.Results:
                    if (!Decision.ResultPrerequisitesMet)
                    {
                        navigationManager.NavigateTo("/options");
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public enum DcidrStep
    {
        Options,
        Criteria,
        CompareOptions,
        CompareCriteria,
        Results
    }
}
