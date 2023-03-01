using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Bug
{
    string testCaseID;
    string stepID;
    string device;
    string description;
    string evidence;
    public Bug(int _testCaseID, string _stepID, string _device, string _description, string _evidence)
    {
        this.testCaseID = _testCaseID.ToString();
        this.stepID = _stepID;
        this.device = _device;
        this.description = _description;
        this.evidence = _evidence;
    }

    public Bug() { }
    public void setStepID(string _stepID) { this.stepID = _stepID; }
    public void setTestcaseID(string testCaseID) { this.testCaseID = testCaseID; }
    public void setDevice(string _device) { this.device = _device; }
    public void setDescription(string _description) { this.description = _description; }
    public void setEvidence(string _evidence) { this.evidence = _evidence; }
    public String getStepID() { return this.stepID; }
    public String getDescription() { return this.description; }
    public String getDevice() { return this.device; }
    public String getTestCaseID() { return this.testCaseID; }
    public String getEvidence() { return this.evidence; }
}