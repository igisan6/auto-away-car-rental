using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace _291_Project_UI
{
    public partial class Form1 : Form
    {
        //list to manage the different windows
        List<Panel> screensList = new List<Panel>();
        MonthCalendar empCalendar = new System.Windows.Forms.MonthCalendar();
        public SqlConnection myConnection;
        public SqlCommand myCommand;
        public SqlDataReader myReader;
        string dateFormat = string.Format("yyyy/MM/dd");    //all dates stored in this format for easy comparison

        public Form1()
        {
            InitializeComponent();
            /* Starting the connection */
            SqlConnection myConnection = new SqlConnection(
                                         "user id=admin;" +         // Username
                                         "password=123;" +          // Password
                                         "server=localhost;" +      // IP for the server
                                                                    //"Trusted_Connection=yes;" +
                                         "database=C291Project; " + // Database to connect to
                                         "connection timeout=30");  // Timeout in seconds 
            try
            {
                myConnection.Open();    // Open connection
                myCommand = new SqlCommand();
                myCommand.Connection = myConnection;    //Link the command stream to the connection
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error");
                this.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //list to hold screens
            screensList.Add(WelcomePage);
            screensList.Add(LoginPage);
            screensList.Add(CustomerPage);
            screensList.Add(EmployeePage);
            screensList[0].BringToFront();

            //calendar config
            empBrowseCalendar.MaxSelectionCount = 60;
            custBrowseCalendar.MaxSelectionCount = 60;

            //populate class comboboxes from database
            myCommand.CommandText = "select distinct Class from cars";
            try
            {
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    empBrowseClass.Items.Add(myReader["Class"].ToString());
                    bookingsClassReq.Items.Add(myReader["Class"].ToString());
                    custBrowseClass.Items.Add(myReader["Class"].ToString());
                    manageClass.Items.Add(myReader["Class"].ToString());
                }
                myReader.Close();
            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }

            //populate branch comboboxes from database
            myCommand.CommandText = "select distinct branchName from branch";
            try
            {
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    empBrowseLocation.Items.Add(myReader["branchName"]);
                    custBrowseLocation.Items.Add(myReader["branchName"]);
                    manageBranch.Items.Add(myReader["branchName"]);
                    returnBranchCB.Items.Add(myReader["branchName"]);
                    branchReportsCB.Items.Add(myReader["branchName"]);
                }
                myReader.Close();
            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }

            //populate model comboboxes from database
            myCommand.CommandText = "select distinct model from cars";
            try
            {
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    empBrowseModel.Items.Add(myReader["model"].ToString());
                    custBrowseModel.Items.Add(myReader["model"].ToString());
                }
                myReader.Close();
            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }
        }

//================== MAIN NAVIGATION ==================
        private void homeButton_Click(object sender, EventArgs e)
        {
            //bring welcome page to front
            screensList[0].BringToFront();
        }
        private void welcomeCustomerButton_Click(object sender, EventArgs e)
        {
            //bring customer panel to front
            screensList[2].BringToFront();
        }
        private void welcomeEmpButton_Click(object sender, EventArgs e)
        {
            //bring login screen to front
            screensList[1].BringToFront();
        }
        private void loginSubmitButton_Click(object sender, EventArgs e)
        {
            //verify employee credentials and navigate to employee screen if accepted
            ///*-----------------
                    if (String.IsNullOrEmpty(loginEID.Text) || String.IsNullOrEmpty(loginPassword.Text))
                    {
                        MessageBox.Show("All fields must be filled!");
                    }
                    else
                    {
                        myCommand.CommandText = "select empID from Employee where "
                            + "empID = " + "'" + loginEID.Text + "'";
                        try
                        {
                            myReader = myCommand.ExecuteReader();
                            if (myReader.Read())
                            {
                                if (loginPassword.Text.Equals("123"))
                                {
            //----------------*/
            screensList[3].BringToFront();
            loginEID.Clear();
            loginPassword.Clear();
            ///*---------------
                                }
                                else
                                {
                                    MessageBox.Show("Incorrect Password!");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Username does not exist!");
                            }
                            myReader.Close();
                        }
                        catch (Exception e3)
                        {
                            MessageBox.Show(e3.ToString(), "Error");
                        }
                    }
            //----------------*/
        }

//================== CUSTOMER BROWSE ==================
        private void custBrowseDateStart_ValueChanged(object sender, EventArgs e)
        {
            //update calendar with selection range
            custBrowseCalendar.SelectionStart = custBrowseDateStart.Value;
            custBrowseCalendar.SelectionEnd = custBrowseDateEnd.Value;
        }
        private void custBrowseDateEnd_ValueChanged(object sender, EventArgs e)
        {
            //update calendar with selection range
            custBrowseCalendar.SelectionStart = custBrowseDateStart.Value;
            custBrowseCalendar.SelectionEnd = custBrowseDateEnd.Value;
        }
        private void custBrowseClearButton_Click(object sender, EventArgs e)
        {
            //clear all fields when clear button clicked
            //reset all comboboxes to their default prompts
            custBrowseClass.SelectedIndex = -1;
            custBrowseClass.Text = "Class";
            custBrowseModel.SelectedIndex = -1;
            custBrowseModel.Text = "Model";
            custBrowseLocation.SelectedIndex = -1;
            custBrowseLocation.Text = "Branch";
            //clear radio buttons
            cbDayRB.Checked = false;
            cbWeekRB.Checked = false;
            cbMonthRB.Checked = false;
            //clear text fields
            custBrowseMinPrice.Clear();
            custBrowseMaxPrice.Clear();
        }
        private void custBrowseSearchButton_Click(object sender, EventArgs e)
        {
            //create data grid with search filters
            //generate (monster) query
            myCommand.CommandText = "select cYear as Year, model as Model, cars.class as Class, dayPrice as 'Daily Price', "
           + "weekPrice as 'Weekly Price', MonthPrice as 'Monthly Price', branchName as Branch, CarID as 'Car ID', "
           + "lateFee as 'Late Fee', differentBranchFee as 'Different Branch Fee' "
           + "from cars, carType, branch "
           + "where cars.Class = carType.Class "
           + "and cars.bID = branch.bID ";
            if (custBrowseClass.SelectedItem != null)
            {
                myCommand.CommandText += "and cars.Class = " + "'" + (string)custBrowseClass.SelectedItem + "' ";
            }
            if (custBrowseModel.SelectedItem != null)
            {
                myCommand.CommandText += "and model = " + "'" + (string)custBrowseModel.SelectedItem + "' ";
            }
            if (custBrowseLocation.SelectedItem != null)
            {
                myCommand.CommandText += "and branch.bID = " + "'" + (string)custBrowseLocation.SelectedItem + "' ";
            }
            if (cbDayRB.Checked)
            {
                if (!String.IsNullOrEmpty(custBrowseMinPrice.Text))
                {
                    myCommand.CommandText += " and dayPrice >= " + custBrowseMinPrice.Text;
                }
                if (!String.IsNullOrEmpty(custBrowseMaxPrice.Text))
                {
                    myCommand.CommandText += " and dayPrice <= " + custBrowseMaxPrice.Text;
                }
            }
            if (cbWeekRB.Checked)
            {
                if (!String.IsNullOrEmpty(custBrowseMinPrice.Text))
                {
                    myCommand.CommandText += " and weekPrice >= " + custBrowseMinPrice.Text;
                }
                if (!String.IsNullOrEmpty(custBrowseMaxPrice.Text))
                {
                    myCommand.CommandText += " and weekPrice <= " + custBrowseMaxPrice.Text;
                }
            }
            if (cbMonthRB.Checked)
            {
                if (!String.IsNullOrEmpty(custBrowseMinPrice.Text))
                {
                    myCommand.CommandText += " and monthPrice >= " + custBrowseMinPrice.Text;
                }
                if (!String.IsNullOrEmpty(custBrowseMaxPrice.Text))
                {
                    myCommand.CommandText += " and monthPrice <= " + custBrowseMaxPrice.Text;
                }
            }
            myCommand.CommandText += "and cars.CarID not in (select carID from booking "
                + "where (" + "'" + custBrowseDateStart.Value.ToString(dateFormat) + "'" + " <= dateTo) "
                + "and (" + "'" + custBrowseDateEnd.Value.ToString(dateFormat) + "'" + " >= dateFrom))";
            
            //update grid
            try
            {

                DataTable bT = new DataTable();
                bT.Rows.Clear();
                bT.Load(myCommand.ExecuteReader());
                //myReader = myCommand.ExecuteReader();
                custBrowseDataGrid.DataSource = bT;

                myReader.Close();
            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }
        }

//================== EMPLOYEE BROWSE ==================
        private void ebDateTimeStart_ValueChanged(object sender, EventArgs e)
        {
            //update selection shown by calendar when date changed
            empBrowseCalendar.SelectionStart = ebDateTimeStart.Value;
            empBrowseCalendar.SelectionEnd = ebDateTimeEnd.Value;
        }
        private void ebDateTimeEnd_ValueChanged(object sender, EventArgs e)
        {
            //update selection shown by calendar when date changed
            empBrowseCalendar.SelectionStart = ebDateTimeStart.Value;
            empBrowseCalendar.SelectionEnd = ebDateTimeEnd.Value;
        }
        private void empBrowseClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            //dropdown menu to select car class on the employee browse screen
            if (empBrowseClass.SelectedItem != null)
            {
                string selectedClass = (string)empBrowseClass.SelectedItem;
            }
        }
        private void empBrowseModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            //dropdown menu for model type on employee browse screen
            if (empBrowseModel.SelectedItem != null)
            {
                string selectedModel = (string)empBrowseModel.SelectedItem;
            }
        }
        private void empBrowseLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            //dropdown menu for branch on employee browse screen
            if (empBrowseLocation.SelectedItem != null)
            {
                string selectedLocation = (string)empBrowseLocation.SelectedItem;
            }
        }
        private void empBrowseClearButton_Click(object sender, EventArgs e)
        {
            //when clear button clicked
            //reset all comboboxes to their default prompts
            empBrowseClass.SelectedIndex = -1;
            empBrowseClass.Text = "Class";
            empBrowseModel.SelectedIndex = -1;
            empBrowseModel.Text = "Model";
            empBrowseLocation.SelectedIndex = -1;
            empBrowseLocation.Text = "Branch";
            //clear radio buttons
            ebDayRB.Checked = false;
            ebWeekRB.Checked = false;
            ebMonthRB.Checked = false;

            //clear text fields
            empBrowseMinPrice.Clear();
            empBrowseMaxPrice.Clear();
        }
        private void empBrowseSearchButton_Click(object sender, EventArgs e)
        {
            //update bookingsDataGrid with the results of the search
            //query string
            myCommand.CommandText = "select cYear as Year, model as Model, cars.class as Class, dayPrice as 'Daily Price', "
            + "weekPrice as 'Weekly Price', MonthPrice as 'Monthly Price', branchName as Branch, CarID as 'Car ID', "
            + "lateFee as 'Late Fee', differentBranchFee as 'Different Branch Fee' "
            + "from cars, carType, branch "
            + "where cars.Class = carType.Class "
            + "and cars.bID = branch.bID ";
            if (empBrowseClass.SelectedItem != null)
            {
                myCommand.CommandText += "and cars.Class = " + "'" + (string)empBrowseClass.SelectedItem + "' ";
            }
            if (empBrowseModel.SelectedItem != null)
            {
                myCommand.CommandText += "and model = " + "'" + (string)empBrowseModel.SelectedItem + "' ";
            }
            if (empBrowseLocation.SelectedItem != null)
            {
                myCommand.CommandText += "and branch.branchName = " + "'" + (string)empBrowseLocation.SelectedItem + "' ";
            }

            if (ebDayRB.Checked)
            {
                if (!String.IsNullOrEmpty(empBrowseMinPrice.Text))
                {
                    myCommand.CommandText += " and dayPrice >= " + empBrowseMinPrice.Text;
                }
                if (!String.IsNullOrEmpty(empBrowseMaxPrice.Text))
                {
                    myCommand.CommandText += " and dayPrice <= " + empBrowseMaxPrice.Text;
                }

            }
            if (ebWeekRB.Checked)
            {
                if (!String.IsNullOrEmpty(empBrowseMinPrice.Text))
                {
                    myCommand.CommandText += " and weekPrice >= " + empBrowseMinPrice.Text;
                }
                if (!String.IsNullOrEmpty(empBrowseMaxPrice.Text))
                {
                    myCommand.CommandText += " and weekPrice <= " + empBrowseMaxPrice.Text;
                }

            }
            if (ebMonthRB.Checked)
            {
                if (!String.IsNullOrEmpty(empBrowseMinPrice.Text))
                {
                    myCommand.CommandText += " and monthPrice >= " + empBrowseMinPrice.Text;
                }
                if (!String.IsNullOrEmpty(empBrowseMaxPrice.Text))
                {
                    myCommand.CommandText += " and monthPrice <= " + empBrowseMaxPrice.Text;
                }

            }
            myCommand.CommandText += "and cars.CarID not in (select carID from booking "
                + "where (" + "'" + ebDateTimeStart.Value.ToString(dateFormat) + "'" + " <= dateTo) "
                + "and (" + "'" + ebDateTimeEnd.Value.ToString(dateFormat) + "'" + " >= dateFrom))";

            //update grid
            try
            {
                DataTable bT = new DataTable();
                bT.Rows.Clear();
                bT.Load(myCommand.ExecuteReader());
                //myReader = myCommand.ExecuteReader();

                bookingsDataGrid.DataSource = bT;

                myReader.Close();

            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }
        }


        //================== EMPLOYEE BOOKING ==================

        private void bookingsClearButton_Click(object sender, EventArgs e)
        {
            //clear all fields within booking
            bookingsFirstName.Clear();
            bookingsLastName.Clear();
            bookingsPhone.Clear();
            bookingsEID.Clear();
            bookingsCarID.Clear();
            dailyPriceRateRB.Checked = false;
            weeklyPriceRateRB.Checked = false;
            monthlyPriceRateRB.Checked = false;

            bookingsClassReq.Text = "Class";
            bookingsClassReq.SelectedIndex = -1;

        }

        private void bookingsCreateButton_Click(object sender, EventArgs e)
        {

            //fetch customer id based on other provided info
            myCommand.CommandText = "select cid from customer where fName = "
                + "'" + bookingsFirstName.Text + "'" + " and lName = "
                + "'" + bookingsLastName.Text + "'" + " and phone = "
                + "'" + bookingsPhone.Text + "'";

            string customerID = "dummy thicc";

            try
            {
                myReader = myCommand.ExecuteReader();

                if (myReader.Read())
                {
                    customerID = myReader.GetValue(0).ToString();

                }

                myReader.Close();
            }

            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }


            //generate booking id
            //select max ID and +1 it
            myCommand.CommandText = "select max(bookID) from booking ";

            string bookingID = "0";


            try
            {
                myReader = myCommand.ExecuteReader();

                if (myReader.Read())
                {
                    bookingID = myReader.GetValue(0).ToString();

                    if (bookingID.Length == 0)
                    {
                        bookingID = "1";
                    }
                    else
                    {
                        //don't hate me
                        bookingID = (1 + Convert.ToInt32(bookingID)).ToString();
                    }

                }

                myReader.Close();
            }

            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }

            //grab branch ID from employee
            myCommand.CommandText = "select bID from employee where empID = "
                + bookingsEID.Text;

            string branchID = "dummy";

            try
            {
                myReader = myCommand.ExecuteReader();

                if (myReader.Read())
                {
                    branchID = myReader.GetValue(0).ToString();

                }

                myReader.Close();
            }

            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }

            string priceRate = "N/A";

            if (dailyPriceRateRB.Checked)
            {
                priceRate = "Daily";
            }
            if (weeklyPriceRateRB.Checked)
            {
                priceRate = "Weekly";
            }
            if (monthlyPriceRateRB.Checked)
            {
                priceRate = "Monthly";
            }


            //create a booking with information from the above fields

            myCommand.CommandText = "insert into booking values ("
            + bookingID + ", "                                  //booking id
            + bookingsEID.Text + ", "                           //employee id
            + customerID + ", "                                 //customer id
            + "'" + ebDateTimeStart.Value.ToString(dateFormat) + "'" + ", "     //date from
            + "'" + ebDateTimeEnd.Value.ToString(dateFormat) + "'" + ", "       //date to
            + "null" + ", "                                     //date received (= null for now)
            + "'" + bookingsClassReq.SelectedItem.ToString() + "'" + ", "      //class requested
            + "'" + priceRate + "'" + ", "                      //price rate (daily/weekly/monthly)
            + bookingsCarID.Text + ", "                         //car ID
            + branchID + ", "                                   //branch from ID
            + "null" + ", "                                     //branch to ID (= null for now)
            + "null"                                            //total cost (null for now)
            + ")";


            //pull the booking trigger

            try
            {

                myCommand.ExecuteNonQuery();
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.ToString(), "Error");
            }

        }


        //================== BOOKING RETURN ==================

        private void processReturnButton_Click(object sender, EventArgs e)
        {
            //retrieve booking from database
            string bookID = returnBookingIDEntry.Text;
            string returnDate = returnDatePicker.Value.ToString(dateFormat);
            string returnTo = (string)returnBranchCB.SelectedItem;
            string dateFrom = "";
            string dateTo = "";
            string reqClass = "";
            string actualClass = "";
            string priceRate = "";
            int carID = 0;
            int fromID = 0;
            int toID = 0;
            int cID = 0;
            int dailyFee = 0;
            int retFee = 0;
            int lateFee = 0;
            int dfBFee = 0;
            int totalCost = 0;

            myCommand.CommandText = "select * from booking "
                + "where bookID = " + bookID;

            try
            {
                myReader = myCommand.ExecuteReader();

                //get information from the booking entry
                while (myReader.Read())
                {

                    dateFrom = myReader["dateFrom"].ToString();
                    dateTo = myReader["dateTo"].ToString();
                    reqClass = myReader["Class"].ToString();
                    priceRate = myReader["priceRate"].ToString();
                    carID = Convert.ToInt32(myReader["carID"]);
                    fromID = Convert.ToInt32(myReader["fromID"]);
                    cID = Convert.ToInt32(myReader["cID"]);

                }

                myReader.Close();
            }

            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }


            //search all bookings with the same year, if count of customer >= 3, mark gold member status
            bool isGoldMember = false;

            //generate query to pull bookings list
            myCommand.CommandText = "select count(cID) as Count from booking "
                + "where cID = " + cID + " "
                + "and dateFrom > " + "'" + dateFrom.Substring(0, 4) + "/01/01" + "' "
                + "group by cID ";

            try
            {
                myReader = myCommand.ExecuteReader();

                //get information from the booking
                if (myReader.Read())
                {

                    if (Convert.ToInt32(myReader["Count"]) >= 3)
                    {
                        isGoldMember = true;
                    }

                }

                myReader.Close();
            }

            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }

            //get the actual class of the car that was rented
            myCommand.CommandText = "select Class from cars "
                + "where carID = " + carID;

            try
            {
                myReader = myCommand.ExecuteReader();

                //get information from the booking
                if (myReader.Read())
                {

                    actualClass = myReader["Class"].ToString();

                }

                myReader.Close();
            }

            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }

            //if customer is a gold member, requested class is treated as actualclass
            if (isGoldMember)
            {
                actualClass = reqClass;
            }

            //pull pricing information from carType

            myCommand.CommandText = "select lateFee as lf, differentBranchFee as dfb, returnFee as rf, ";

            if (priceRate == "Daily")
            {
                myCommand.CommandText += "dayPrice as ppd ";
            }
            if (priceRate == "Weekly")
            {
                myCommand.CommandText += "weekPrice as ppd ";
            }
            if (priceRate == "Monthly")
            {
                myCommand.CommandText += "monthPrice as ppd ";
            }

            myCommand.CommandText += "from carType "
                + "where Class = " + "'" + actualClass + "'";

            try
            {
                myReader = myCommand.ExecuteReader();

                if (myReader.Read())
                {
                    dailyFee = Convert.ToInt32(myReader["ppd"]);
                    lateFee = Convert.ToInt32(myReader["lf"]);
                    dfBFee = Convert.ToInt32(myReader["dfb"]);
                    retFee = Convert.ToInt32(myReader["rf"]);

                }

                myReader.Close();
            }

            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }

            //calculate number of days
            DateTime startDate = DateTime.Parse(dateFrom);
            DateTime endDate = DateTime.Parse(returnDate);

            int numDays = (int)((endDate - startDate).TotalDays);

            //if the actual return date is later than proposed, apply late fee
            //1 = late, -1 = early, 0 = proper return day
            if (String.Compare(returnDate, dateTo) < 1)
            {
                lateFee = 0;
            }

            // Convert Branch Name to Branch ID
            if ((string)returnBranchCB.SelectedItem == "Edmonton Cars"){toID = 1;}
            if ((string)returnBranchCB.SelectedItem == "Calgary Rental"){toID = 2;}
            if ((string)returnBranchCB.SelectedItem == "Vancouver Enterprise"){toID = 3;}

            //reset different branch fee if returned to the same
            if (fromID == toID)
            {
                dfBFee = 0;
            } else {
                //otherwise associate the car with the new branch

                myCommand.CommandText = "Update Cars Set "
                + "bID = " + toID
                + "Where carID = " + carID;

                try
                {
                    myCommand.ExecuteNonQuery();
                }
                catch (Exception e5)
                {
                    MessageBox.Show(e5.ToString(), "Manage Cars Execution Error.");
                }
            }

            //do the same if customer is a gold member
            if (isGoldMember)
            {
                dfBFee = 0;
            }

            totalCost = (retFee + (numDays * dailyFee) + lateFee + dfBFee);

            //show price
            MessageBox.Show("Return complete!\n" +
                "Cost Breakdown:\n" +
                "-----------------------\n" +
                "Base Price: \t$" + retFee + "\n" +
                "Daily rate: \t$" + dailyFee + "\n" +
                "Number of days: \t" + numDays + "\n" +
                "Base cost: \t$" + (retFee + (numDays * dailyFee)) + "\n\n" +
                "Additional fees:\n" +
                "Late: \t\t$" + lateFee + "\n" +
                "Different Branch: \t$" + dfBFee + "\n" +
                "Total: \t\t$" + totalCost
                );

            //update booking entry with the return branch, the date returned, and the total cost


            myCommand.CommandText = "Update booking Set "
                + "dateReceived = " + "'" + returnDatePicker.Value.ToString(dateFormat) + "', "
                + "toID = " + toID + ", "
                + "totalCost = " + totalCost + " "
                + "Where bookID = " + bookID;

            try
            {
                myCommand.ExecuteNonQuery();
            }
            catch (Exception e5)
            {
                MessageBox.Show(e5.ToString(), "Manage Cars Execution Error.");
            }
        }


        //================== MANAGE CARS ==================

        private void manageModel_TextChanged(object sender, EventArgs e)
        {
            //model field from the manager page
            //will auto-update based on specified car ID if search is clicked, or can be manually entered
            //shouldnt need any code here, just for info
        }

        private void manageYear_TextChanged(object sender, EventArgs e)
        {
            //year field from the manager page
            //will auto-update based on specified car ID if search is clicked, or can be manually entered
            //shouldnt need any code here either
        }

        private void createEntryButton_Click(object sender, EventArgs e)
        {

            //generate carID
            myCommand.CommandText = "select max(carID) from cars ";
            string cID = "0";

            try
            {
                myReader = myCommand.ExecuteReader();

                if (myReader.Read())
                {
                    cID = myReader.GetValue(0).ToString();

                    if (cID.Length == 0)
                    {
                        cID = "1";
                    }
                    else
                    {
                        cID = (1 + Convert.ToInt32(cID)).ToString();
                    }
                }

                myReader.Close();
            }

            catch (Exception e4)
            {
                MessageBox.Show(e4.ToString(), "Generating carID Error");
            }

            int branchID = 1;
            if ((string)manageBranch.SelectedItem == "Edmonton Cars")
            {
                branchID = 1;
            }
            if ((string)manageBranch.SelectedItem == "Calgary Rental")
            {
                branchID = 2;
            }
            if ((string)manageBranch.SelectedItem == "Vancouver Enterprise")
            {
                branchID = 3;
            }

                //insert input into cars table

            myCommand.CommandText = "insert into cars values ("
            + cID + ", "                                                 //carID
            + "'" + manageModel.Text + "'" + ", "                        //model
            + manageYear.Text + ", "                                     //cYear
            + "0" + ", "                                                 //mileage (default is zero)
            + branchID + ", "                                            //bID
            + "'" + manageClass.SelectedItem.ToString() + "'"            //class
            + ")";

            //create car
            try
            {
                myCommand.ExecuteNonQuery();
            }
            catch (Exception e5)
            {
                MessageBox.Show(e5.ToString(), "Manage Cars Execution Error");
            }
        }

        private void manageSearchButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(manageCarID.Text))
            {
                MessageBox.Show("Car ID field must be filled!");
            }
            else
            {
                myCommand.CommandText = "select * from Cars where "
                    + "carID = " + manageCarID.Text;
                try
                {
                    myReader = myCommand.ExecuteReader();
                    if (myReader.Read())
                    {

                        manageModel.Text = myReader["model"].ToString();
                        manageYear.Text = myReader["cYear"].ToString();
                        manageClass.Text = "";
                        manageClass.SelectedIndex = manageClass.FindString(manageClass.SelectedText = myReader["Class"].ToString());
                        manageBranch.Text = "";
                        String branchID = myReader["bID"].ToString();
                        if (branchID == "1")
                        {
                            manageBranch.Text = "Edmonton Cars";
                            manageBranch.SelectedIndex = Convert.ToInt32(branchID);
                        }
                        if (branchID == "2")
                        {
                            manageBranch.Text = "Calgary Rental";
                            manageBranch.SelectedIndex = Convert.ToInt32(branchID);
                        }
                        if (branchID == "3")
                        {
                            manageBranch.Text = "Vancouver Enterprise";
                            manageBranch.SelectedIndex = Convert.ToInt32(branchID);
                        }
                    }

                    myReader.Close();
                }
                catch (Exception e3)
                {
                    MessageBox.Show(e3.ToString(), "Error");
                }
            }

        }
        private void updateEntryButton_Click(object sender, EventArgs e)
        {
            //takes information from the fields and applies it to the selected car ID
            if (String.IsNullOrEmpty(manageCarID.Text))
            {
                MessageBox.Show("Car ID field must be filled!");
            }
            else
            {

                int bID = 0;
                if (manageBranch.Text == "Edmonton Cars"){bID = 1;}
                if (manageBranch.Text == "Calgary Rental"){bID = 2;}
                if (manageBranch.Text == "Vancouver Enterprise") {bID = 3;}

                myCommand.CommandText = "Update Cars Set "
                + "model = '" + manageModel.Text + "', "
                + "cYear = " + manageYear.Text + ", "
                + "bID = " + bID + ", "
                + "Class = '" + manageClass.Text + "' "
                + "Where carID = " + manageCarID.Text;

                //Update car
                try
                {
                    myCommand.ExecuteNonQuery();
                }
                catch (Exception e5)
                {
                    MessageBox.Show(e5.ToString(), "Manage Cars Execution Error.");
                }
            }
        }
        private void deleteEntryButton_Click(object sender, EventArgs e)
        {
            //removes the database entry associated with the car ID
            if (String.IsNullOrEmpty(manageCarID.Text))
            {
                MessageBox.Show("Car ID field must be filled!");
            }
            else
            {
                myCommand.CommandText = "delete from Cars where "
                    + "carID = " + manageCarID.Text;
                //Delete car
                try
                {
                    myCommand.ExecuteNonQuery();
                }
                catch (Exception e5)
                {
                    MessageBox.Show(e5.ToString(), "Manage Cars Execution Error");
                }
            }
        }

        private void manageClear_Click(object sender, EventArgs e)
        {
            //clears all fields in the window
            manageCarID.Clear();
            manageModel.Clear();
            manageYear.Clear();

            manageClass.SelectedIndex = -1;
            manageClass.Text = "Class";

            manageBranch.SelectedIndex = -1;
            manageBranch.Text = "Branch";
        }


//================== REPORTS ==================

        //================== CAR REPORTS ==================
        private void carReportTimeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (carReportTimeCB.SelectedItem.ToString() != "all time")
            {
                carReportDatePicker.Visible = true;
            }
            else
            {
                carReportDatePicker.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //apply query
            myCommand.CommandText = "Select cars.Model, totalCars.total "
            + "From cars, (select carID, ";

            if (carReportCB.SelectedItem.ToString() == "most popular")
            {
                myCommand.CommandText += "count (*) ";
            }
            else
            {
                myCommand.CommandText += "sum(totalCost) ";
            }

            myCommand.CommandText += "as total "
           + "From booking ";

            if (carReportTimeCB.SelectedItem.ToString() == "year of")
            {
                myCommand.CommandText += "where dateFrom >= " + "'" + carReportDatePicker.Value.ToString("yyyy") + "/01/01' "
                + "and dateFrom <= " + "'" + (Convert.ToInt32(carReportDatePicker.Value.ToString("yyyy")) + 1) + "/01/01' ";

            }
            if (carReportTimeCB.SelectedItem.ToString() == "month of")
            {
                myCommand.CommandText += "where dateFrom >= " + "'" + carReportDatePicker.Value.ToString("yyyy/MM") + "/01' "
                + "and dateFrom < ";

                //if december, increment year and reset month to 1
                if ((Convert.ToInt32(carReportDatePicker.Value.ToString("MM")) + 1).ToString("00") == "13")
                {
                    myCommand.CommandText += "'" + (Convert.ToInt32(carReportDatePicker.Value.ToString("yyyy")) + 1) + "/01";
                }
                else
                {
                    //otherwise increment month normally
                    myCommand.CommandText += "'" + Convert.ToInt32(carReportDatePicker.Value.ToString("yyyy")) + "/"
                    + (Convert.ToInt32(carReportDatePicker.Value.ToString("MM")) + 1).ToString("00");
                }

                //checking from first day of 1 month to first day of next month
                myCommand.CommandText += ""
                    + "/01' ";
            }

            myCommand.CommandText += "Group by carID) as totalCars "
            + "Where cars.carID = totalCars.carID order by totalCars.total desc";

            try
            {

                DataTable bT = new DataTable();
                bT.Rows.Clear();
                bT.Load(myCommand.ExecuteReader());

                carReportsDataGrid.DataSource = bT;

                myReader.Close();

            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }
        }


        //================== CUSTOMER REPORTS ==================

        private void customerReportsButton_Click(object sender, EventArgs e)
        {

            if (customerReportsCB.SelectedItem.ToString() == "customers")
            {

                myCommand.CommandText = "select fName + ' ' + lName as Name "
                + "from customer "
                + "Where cID in (select temp.cID "
                + "From (select cID, count(*) as total "
                + "From booking "
                + "Group by cID "
                + "Having count(*) >= " + customerReportsN.Text + ") as temp)";

                try
                {

                    DataTable bT = new DataTable();

                    bT.Rows.Clear();
                    bT.Load(myCommand.ExecuteReader());

                    customerReportsDataGrid.DataSource = bT;

                    myReader.Close();

                }
                catch (Exception e3)
                {
                    MessageBox.Show(e3.ToString(), "Error");
                }
            }

            if (customerReportsCB.SelectedItem.ToString() == "percentage of customers")
            {
                myCommand.CommandText = "Select((numerator.num1* 100)/denominator.num2) as Percentage "
                    + "From(select count(*) as num2 "
                    + "From Customer) as denominator, (select count(*) as num1 "
                    + "From(select cID, count(*) as nTotal "
                    + "From booking "
                    + "Group by cID "
                    + "Having count(*) >= " + customerReportsN.Text + ") as poop) as numerator";

                try
                {
                    DataTable bT = new DataTable();
                    bT.Rows.Clear();
                    bT.Load(myCommand.ExecuteReader());

                    customerReportsDataGrid.DataSource = bT;

                    myReader.Close();

                }
                catch (Exception e3)
                {
                    MessageBox.Show(e3.ToString(), "Error");
                }
            }


            if (customerReportsCB.SelectedItem.ToString() == "percentage of revenue generated by customers")
            {
                myCommand.CommandText = "Select ((numerator.cTotal * 100)/denominator.total) as Percentage "
                    + "From (select sum(totalCost) as total "
                    + "From booking) as denominator, (select sum(totalCost) as cTotal "
                    + "From Booking, (select cID, count(*) as nBookings "
                    + "From booking "
                    + "Group by cID "
                    + "Having count(*) >= " + customerReportsN.Text + ") as temp "
                    + "Where booking.cID = temp.cID) as numerator ";

                try
                {
                    DataTable bT = new DataTable();

                    bT.Rows.Clear();
                    bT.Load(myCommand.ExecuteReader());

                    customerReportsDataGrid.DataSource = bT;

                    myReader.Close();

                }
                catch (Exception e3)
                {
                    MessageBox.Show(e3.ToString(), "Error");
                }
            }

        }

        //================== Branch REPORTS ==================

        private void branchReportsButton_Click(object sender, EventArgs e)
        {

            int branchID = 0;

            if ((string)branchReportsCB.SelectedItem == "Edmonton Cars")
            {
                branchID = 1;
            }
            if ((string)branchReportsCB.SelectedItem == "Calgary Rental")
            {
                branchID = 2;
            }
            if ((string)branchReportsCB.SelectedItem == "Vancouver Enterprise")
            {
                branchID = 3;
            }


            myCommand.CommandText = "Select fName + ' ' + lName, counter.total "
                + "From Employee, (select empID, ";
            
            if (branchReportsOptionsCB.SelectedItem.ToString() == "number of sales")
            {
                myCommand.CommandText += "count(*) as total ";
            }
            if (branchReportsOptionsCB.SelectedItem.ToString() == "total revenue generated")
            {
                myCommand.CommandText += "sum(totalCost) as total ";
            }

            myCommand.CommandText += "From Booking ";

            if (branchReportsCB.SelectedItem.ToString() != "Everywhere")
            {
                myCommand.CommandText += "Where fromID = " + branchID + " ";
            }

            myCommand.CommandText += "Group by empID) as counter "
                + "Where Employee.empID = counter.empID "
                + "Order by counter.total desc ";

            try
            {
                DataTable bT = new DataTable();
                bT.Rows.Clear();
                bT.Load(myCommand.ExecuteReader());

                branchReportsDataGrid.DataSource = bT;

                myReader.Close();

            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }
        }

        //================== Booking Search ==================
        //provides user with various ways to view bookings
        private void bookingsReportsButton_Click(object sender, EventArgs e)
        {

            if (bookingsReportsCB.SelectedItem.ToString() == "all bookings")
            {
                myCommand.CommandText = "select * from booking";
            }
            if (bookingsReportsCB.SelectedItem.ToString() == "completed bookings")
            {
                myCommand.CommandText = "select * from booking where dateReceived is not null";
            }
            if (bookingsReportsCB.SelectedItem.ToString() == "non-completed bookings")
            {
                myCommand.CommandText = "select * from booking where dateReceived is null";
            }
            if (bookingsReportsCB.SelectedItem.ToString() == "bookings that were returned late")
            {
                myCommand.CommandText = "select * from booking where dateTo < dateReceived";
            }
            if (bookingsReportsCB.SelectedItem.ToString() == "bookings that were returned to a different branch")
            {
                myCommand.CommandText = "select * from booking where toID != fromID";
            }

            try
            {

                DataTable bT = new DataTable();
                bT.Rows.Clear();
                bT.Load(myCommand.ExecuteReader());

                bookingsReportsDataGrid.DataSource = bT;

                myReader.Close();

            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }
        }

        //================== Custom Query ==================

        private void customQueryButton_Click(object sender, EventArgs e)
        {
            myCommand.CommandText = customQueryField.Text;

            try
            {
                DataTable bT = new DataTable();

                bT.Rows.Clear();
                bT.Load(myCommand.ExecuteReader());

                customQueryDataGrid.DataSource = bT;

                myReader.Close();

            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString(), "Error");
            }
        }
    }
}