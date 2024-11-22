using System;
using System.Collections;
using System.Collections.Generic;
// using System.ComponentModel;
using System.IO;
// using System.Runtime.InteropServices;

using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
using NXOpen.Assemblies;
using NXOpen.BlockStyler;

//------------------------------------------------------------------------------
//Represents Block Styler application class
//------------------------------------------------------------------------------
public class AttributeUnlock
{
    //class members
    private static Session theSession = Session.GetSession();
    private static UI theUI = UI.GetUI();
    private string theDlxFileName;
    private NXOpen.BlockStyler.BlockDialog theDialog;
    private NXOpen.BlockStyler.Enumeration enum0selectwork;// Block type: Enumeration
    private NXOpen.BlockStyler.Group group;// Block type: Group
    private NXOpen.BlockStyler.Label label0;// Block type: Label
    private NXOpen.BlockStyler.Label label01;// Block type: Label

    readonly String getdir = Path.GetDirectoryName(theSession.ExecutingJournal);
    readonly String getfile = Path.GetFileName(theSession.ExecutingJournal);
    private ListingWindow lw = theSession.ListingWindow;
    private Part workPart = theSession.Parts.Work;
    private Part displayPart = theSession.Parts.Display;
    private List<Part> targetParts = new List<Part>();


    
    //------------------------------------------------------------------------------
    //Constructor for NX Styler class
    //------------------------------------------------------------------------------
    public AttributeUnlock()
    {
        try
        {
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theDlxFileName = getdir+ "\\dlx\\"+ getfile.Substring(0, getfile.Length-2) +"dlx";
            theDialog = theUI.CreateDialog(theDlxFileName);
            theDialog.AddApplyHandler(new NXOpen.BlockStyler.BlockDialog.Apply(apply_cb));
            theDialog.AddOkHandler(new NXOpen.BlockStyler.BlockDialog.Ok(ok_cb));
            theDialog.AddUpdateHandler(new NXOpen.BlockStyler.BlockDialog.Update(update_cb));
            theDialog.AddInitializeHandler(new NXOpen.BlockStyler.BlockDialog.Initialize(initialize_cb));
            theDialog.AddDialogShownHandler(new NXOpen.BlockStyler.BlockDialog.DialogShown(dialogShown_cb));
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            throw ex;
        }
    }
    //------------------------------- DIALOG LAUNCHING ---------------------------------
    //
    //    Before invoking this application one needs to open any part/empty part in NX
    //    because of the behavior of the blocks.
    //
    //    Make sure the dlx file is in one of the following locations:
    //        1.) From where NX session is launched
    //        2.) $UGII_USER_DIR/application
    //        3.) For released applications, using UGII_CUSTOM_DIRECTORY_FILE is highly
    //            recommended. This variable is set to a full directory path to a file 
    //            containing a list of root directories for all custom applications.
    //            e.g., UGII_CUSTOM_DIRECTORY_FILE=$UGII_BASE_DIR\ugii\menus\custom_dirs.dat
    //
    //    You can create the dialog using one of the following way:
    //
    //    1. Journal Replay
    //
    //        1) Replay this file through Tool->Journal->Play Menu.
    //
    //    2. USER EXIT
    //
    //        1) Create the Shared Library -- Refer "Block UI Styler programmer's guide"
    //        2) Invoke the Shared Library through File->Execute->NX Open menu.
    //
    //------------------------------------------------------------------------------
    public static void Main()
    {
        if (theSession.Parts.Work == null)
        {
            theUI.NXMessageBox.Show("", NXMessageBox.DialogType.Warning, "Work Part가 필요합니다.");
        }
        else
        {
            AttributeUnlock theAttributeUnlock = null;
            try
            {
                theAttributeUnlock = new AttributeUnlock();
                // The following method shows the dialog immediately
                theAttributeUnlock.Launch();
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            finally
            {
                if(theAttributeUnlock != null)
                    theAttributeUnlock.Dispose();
                    theAttributeUnlock = null;
            }
        }
    }
    //------------------------------------------------------------------------------
    // This method specifies how a shared image is unloaded from memory
    // within NX. This method gives you the capability to unload an
    // internal NX Open application or user  exit from NX. Specify any
    // one of the three constants as a return value to determine the type
    // of unload to perform:
    //
    //
    //    Immediately : unload the library as soon as the automation program has completed
    //    Explicitly  : unload the library from the "Unload Shared Image" dialog
    //    AtTermination : unload the library when the NX session terminates
    //
    //
    // NOTE:  A program which associates NX Open applications with the menubar
    // MUST NOT use this option since it will UNLOAD your NX Open application image
    // from the menubar.
    //------------------------------------------------------------------------------
     public static int GetUnloadOption(string arg)
    {
        //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);
         return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);
        // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
    }
    
    //------------------------------------------------------------------------------
    // Following method cleanup any housekeeping chores that may be needed.
    // This method is automatically called by NX.
    //------------------------------------------------------------------------------
    public static void UnloadLibrary(string arg)
    {
        try
        {
            //---- Enter your code here -----
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
    }
    
    //------------------------------------------------------------------------------
    //This method launches the dialog to screen
    //------------------------------------------------------------------------------
    public NXOpen.BlockStyler.BlockDialog.DialogResponse Launch()
    {
        NXOpen.BlockStyler.BlockDialog.DialogResponse dialogResponse = NXOpen.BlockStyler.BlockDialog.DialogResponse.Invalid;
        try
        {
            dialogResponse = theDialog.Launch();
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return dialogResponse;
    }
    
    //------------------------------------------------------------------------------
    //Method Name: Dispose
    //------------------------------------------------------------------------------
    public void Dispose()
    {
        if(theDialog != null)
        {
            theDialog.Dispose();
            theDialog = null;
        }
    }
    
    //------------------------------------------------------------------------------
    //---------------------Block UI Styler Callback Functions--------------------------
    //------------------------------------------------------------------------------
    
    //------------------------------------------------------------------------------
    //Callback Name: initialize_cb
    //------------------------------------------------------------------------------
    public void initialize_cb()
    {
        try
        {
            enum0selectwork = (NXOpen.BlockStyler.Enumeration)theDialog.TopBlock.FindBlock("enum0selectwork");
            group = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("group");
            label0 = (NXOpen.BlockStyler.Label)theDialog.TopBlock.FindBlock("label0");
            label01 = (NXOpen.BlockStyler.Label)theDialog.TopBlock.FindBlock("label01");
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
    }
    
    //------------------------------------------------------------------------------
    //Callback Name: dialogShown_cb
    //This callback is executed just before the dialog launch. Thus any value set 
    //here will take precedence and dialog will be launched showing that value. 
    //------------------------------------------------------------------------------
    public void dialogShown_cb()
    {
        try
        {
            //---- Enter your callback code here -----
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
    }
    
    //------------------------------------------------------------------------------
    //Callback Name: apply_cb
    //------------------------------------------------------------------------------
    public int apply_cb()
    {
        int errorCode = 0;
        try
        {
            //---- Enter your callback code here -----

            Part earlyPart = workPart;

            if (enum0selectwork.ValueAsString != "Work Part Only" && workPart.ComponentAssembly.RootComponent != null)
            {
                RecursiveCollectChild(workPart.ComponentAssembly.RootComponent, ref targetParts);
            }
            targetParts.Add(workPart);


            lw.Open();
            foreach (Part thePart in targetParts)
            {
                var attributeInfo = thePart.GetUserAttributes();
                foreach (NXObject.AttributeInformation attr in attributeInfo)
                {
                    if (attr.OwnedBySystem == false)
                    {
                        if (attr.Locked == true)
                        {
                            try
                            {
                                thePart.SetUserAttributeLock(attr.Title, NXObject.AttributeType.Any, false);
                                lw.WriteLine("Attribute Unlocked 성공 : " + thePart.Name + " - @" + attr.Title);
                            }
                            catch
                            {
                                lw.WriteLine("@실패 : " + thePart.Name + " - @" + attr.Title);
                            }
                        }
                    }
                }
            }
            lw.Close();

        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            errorCode = 1;
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return errorCode;
    }
    
    //------------------------------------------------------------------------------
    //Callback Name: update_cb
    //------------------------------------------------------------------------------
    public int update_cb( NXOpen.BlockStyler.UIBlock block)
    {
        try
        {
            if(block == enum0selectwork)
            {
            //---------Enter your code here-----------
            }
            else if(block == label0)
            {
            //---------Enter your code here-----------
            }
            else if(block == label01)
            {
            //---------Enter your code here-----------
            }
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return 0;
    }
    
    //------------------------------------------------------------------------------
    //Callback Name: ok_cb
    //------------------------------------------------------------------------------
    public int ok_cb()
    {
        int errorCode = 0;
        try
        {
            errorCode = apply_cb();
            //---- Enter your callback code here -----
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            errorCode = 1;
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return errorCode;
    }
    
    //------------------------------------------------------------------------------
    //Function Name: GetBlockProperties
    //Returns the propertylist of the specified BlockID
    //------------------------------------------------------------------------------
    public PropertyList GetBlockProperties(string blockID)
    {
        PropertyList plist =null;
        try
        {
            plist = theDialog.GetBlockProperties(blockID);
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return plist;
    }

    private void RecursiveCollectChild(NXOpen.Assemblies.Component parentComponent , ref List<Part> theParts)
    {
        foreach (NXOpen.Assemblies.Component child in parentComponent.GetChildren())
        {
            Part part = child.Prototype as Part;
            if (!child.IsSuppressed && part != null && !theParts.Contains(part))
            {             
            theParts.Add(part);
            RecursiveCollectChild(child, ref theParts);
            }
        }
    }
    
}
