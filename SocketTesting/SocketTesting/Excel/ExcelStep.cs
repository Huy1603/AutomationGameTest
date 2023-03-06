using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Numerics;
using System.Diagnostics;
using System.Reflection;
using NPOI.SS.Formula.Functions;

public class ExcelStep
{
    private static string paramSeparator = ",";

    private string stepID;
    private string action;
    private string[] Params;
    private string evidence;
    private string driver;

    public string getStepID()
    {
        return stepID;
    }

    public void setStepID(string _stepID)
    {
        this.stepID = _stepID;
    }

    public string getAction()
    {
        return action;
    }

    public void setAction(string _action)
    {
        this.action = _action;
    } 

    public string getDriver()
    {
        return driver;
    }

    public void setDriver(string _driver)
    {
        this.driver = _driver;
    }

    public void setParam(string _params)
    {
        this.Params = _params.Split(paramSeparator);
    }

    public string[] getParam()
    {
        return this.Params;
    }

    public string getEvidence()
    {
        return evidence;
    }

    public void setEvidence()
    {
        try
        {
            ADBObject obj = ADBDeviceSelector.GetADBObject(this.driver);
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("captureScreen");
            this.evidence = method.Invoke(obj, null).ToString();
        }
        catch (Exception ex)
        {
            throw new Exception($"can not set evidence for {driver}");
        }
    }

    public ExcelStep cloning()
    {
        ExcelStep clone = new ExcelStep();
        clone.stepID = this.stepID;
        clone.action = this.action;
        clone.driver = this.driver;
        if (Params != null) clone.Params = this.Params;
        return clone;
    }

    public void executeWithParam()
    {
        ADBObject obj = ADBDeviceSelector.GetADBObject(this.driver);
        Type type = obj.GetType();
        Type[] paramsType = null;
        paramsType = new Type[Params.Length];
        Array.Fill(paramsType, typeof(string));
        MethodInfo method;
        try
        {
            method = type.GetMethod(action, paramsType);
        }
        catch (Exception ex)
        {
            throw new Exception($"input might wrong, can not find method {action} with params {string.Join(paramSeparator, Params)} at step {stepID}.");
        }
        try
        {
            method.Invoke(obj, Params);
        }
        catch (Exception ex)
        {
            setEvidence();
            throw new Exception(this.setStepID + TestCaseSheet.bugStepIDAndDescriptionSeparator + ex);
        }
    }

    public void executeWithoutParam()
    {
        ADBObject obj = ADBDeviceSelector.GetADBObject(this.driver);
        Type type = obj.GetType();
        Type[] paramsType = null;
        MethodInfo method;
        try
        {
            method = type.GetMethod(action);
        }
        catch (Exception ex)
        {
            throw new Exception($"input might wrong, can not find method {action} at step {stepID}.");
        }
        try
        {
            method.Invoke(obj, Params);
        }
        catch (Exception ex)
        {
            setEvidence();
            throw new Exception(this.setStepID + TestCaseSheet.bugStepIDAndDescriptionSeparator + ex);
        }
    }

    public void execute()
    {
        if(Params == null) executeWithoutParam();
        else executeWithParam();
    }

    public override string ToString()
    {
        if(Params != null) return $"Step{{ id: {this.stepID}, driver: {this.driver}, action: {this.action}, params: {string.Join(paramSeparator, Params)}";
        else return $"Step{{ id: {this.stepID}, driver: {this.driver}, action: {this.action}}}";
    }
}
