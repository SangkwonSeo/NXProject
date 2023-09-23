using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using NXOpen;
using NXOpenUI;
using NXOpen.Utilities;
using NXOpen.UF;

static class make_bounding_block_of_selected_body_relative_to_wcs
{
    private static Session s = Session.GetSession();
    private static UFSession ufs = UFSession.GetUFSession();
    private static ListingWindow lw = s.ListingWindow();

    public static void Main()
    {
        NXOpen.Tag a_body = NXOpen.Tag.Null;
        NXOpen.Tag csys = NXOpen.Tag.Null;
        NXOpen.Tag target = NXOpen.Tag.Null;
        NXOpen.Tag blockFeature = NXOpen.Tag.Null;
        NXopen
        double[] min_corner = new double[2];
        double[] directions = new double[2,2];
        double[] distances = new double[2];
        string[] edge_len = new string[2];

        while (select_a_body(ref a_body) == Selection.Response.Ok)
        {
            ufs.Csys.AskWcs(csys);

            ufs.Modl.AskBoundingBoxExact(a_body, csys, min_corner, directions, distances);

            lw.Open();

            lw.WriteLine("Min_corner: " + min_corner[0].ToString() + ", " + min_corner[1].ToString() + ", " + min_corner[2].ToString() + ", ");

            lw.WriteLine("X direction: " + directions[0, 0].ToString() + ", " + directions[0, 1].ToString() + ", " + directions[0, 2].ToString() + ", ");
            lw.WriteLine("X distance: " + distances[0].ToString());

            lw.WriteLine("Y direction: " + directions[1, 0].ToString() + ", " + directions[1, 1].ToString() + ", " + directions[1, 2].ToString() + ", ");
            lw.WriteLine("Y distance: " + distances[1].ToString());

            lw.WriteLine("Z direction: " + directions[2, 0].ToString() + ", " + directions[2, 1].ToString() + ", " + directions[2, 2].ToString() + ", ");
            lw.WriteLine("Z distance: " + distances[2].ToString());

            edge_len[0] = distances[0].ToString();
            edge_len[1] = distances[1].ToString();
            edge_len[2] = distances[2].ToString();

            ufs.Modl.CreateBlock(FeatureSigns.Nullsign, target, min_corner, edge_len, blockFeature);
        }
    }



    public static Selection.Response select_a_body(ref NXOpen.Tag a_body)
    {
        string message = "Select aa body"; // 상태표시줄
        string title = "Select a body"; // 메뉴 상단
        int scope = UFConstants.UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY;
        int response;

        NXOpen.Tag view;
        double[] cursor = new double[3];
        UFUi.SelInitFnT ip = body_init_proc;

        ufs.Ui.LockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);

        try
        {
            ufs.Ui.SelectWithSingleDialog(message, title, scope, ip, null/* TODO Change to default(_) if this is not a reference type */, response, a_body, cursor, view);
        }
        finally
        {
            ufs.Ui.UnlockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);
        }

        if (response != UFConstants.UF_UI_OBJECT_SELECTED & response != UFConstants.UF_UI_OBJECT_SELECTED_BY_NAME)
            return Selection.Response.Cancel;
        else
        {
            ufs.Disp.SetHighlight(a_body, 0);
            return Selection.Response.Ok;
        }
    }

    

    public static int body_init_proc(IntPtr select_, IntPtr userdata)
    {
        int num_triples = 1;
        UFUi.Mask[] mask_triples = new UFUi.Mask[1];
        mask_triples[0].object_type = UFConstants.UF_solid_type;
        mask_triples[0].object_subtype = UFConstants.UF_solid_body_subtype;
        mask_triples[0].solid_type = UFConstants.UF_UI_SEL_FEATURE_BODY;

        ufs.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, num_triples, mask_triples);
        return UFConstants.UF_UI_SEL_SUCCESS;
    }

    public static int GetUnloadOption(string dummy)
    {
        GetUnloadOption = UFConstants.UF_UNLOAD_IMMEDIATELY;
    }
}
