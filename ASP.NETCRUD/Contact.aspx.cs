using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASP.NETCRUD
{
    public partial class Contact : System.Web.UI.Page
    {
        SqlConnection sqlCon = new SqlConnection(@"Data Source=.\SQLEXPRESS; initial catalog=ASPCRUD; integrated security=true;");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnDelete.Enabled = false;
                fillGridView();
            }

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();

        }

        public void Clear()
        {
            hfContactID.Value = "";
            txtName.Text = txtMobile.Text = txtAddress.Text = "";
            lblSuccessMessage.Text = lblErrorMessage.Text = "";
            btnSave.Text = "Saved";
            btnDelete.Enabled = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            SqlCommand sqlCmd = new SqlCommand("ContactCreateorUpdate", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@ContactID", (hfContactID.Value=="" ?0:Convert.ToInt32(hfContactID.Value))); 
            sqlCmd.Parameters.AddWithValue("@Name",txtName.Text.Trim());
            sqlCmd.Parameters.AddWithValue("@Mobile", txtMobile.Text.Trim());
            sqlCmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
            sqlCmd.ExecuteNonQuery();
            sqlCon.Close();
            string ContactId = hfContactID.Value;
            Clear();
            if (ContactId == "")
            {
                lblSuccessMessage.Text = "Saved Succesfully";
            }
            else
            {
                lblSuccessMessage.Text = "Updated Succesfully";
            }
            fillGridView();
        }
        void fillGridView()
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            SqlDataAdapter sqlDa = new SqlDataAdapter("ContactViewAll", sqlCon);
            sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dtbl = new DataTable();
            sqlDa.Fill(dtbl);
            sqlCon.Close();
            gvContact.DataSource = dtbl;
            gvContact.DataBind();
        }
        protected void lnk_Onclick(object sender, EventArgs e)
        {
            int contactID = Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            SqlDataAdapter sqlDa = new SqlDataAdapter("ContactViewByID", sqlCon);
            sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sqlDa.SelectCommand.Parameters.AddWithValue("@ContactID", contactID);
            DataTable dtbl = new DataTable();
            sqlDa.Fill(dtbl);
            sqlCon.Close();
            hfContactID.Value = contactID.ToString();
            txtName.Text = dtbl.Rows[0]["Name"].ToString();
            txtMobile.Text = dtbl.Rows[0]["Mobile"].ToString();
            txtAddress.Text = dtbl.Rows[0]["Address"].ToString();
            btnSave.Text = "Update";
            btnDelete.Enabled = true;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            SqlCommand sqlCmd = new SqlCommand("ContactDeleteByID", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@ContactID", Convert.ToInt32(hfContactID.Value));
            sqlCmd.ExecuteNonQuery();
            Clear();
            fillGridView();
            lblSuccessMessage.Text = "Deleted succesfully";
            sqlCon.Close();
        }
    }
}