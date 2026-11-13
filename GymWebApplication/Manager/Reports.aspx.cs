using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GymWebApplication.Manager
{
    public partial class Reports : System.Web.UI.Page
    {
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager
                    .ConnectionStrings["DefaultConnection"]
                    .ConnectionString;
            }
        }


        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsManagerLoggedIn())
            {
                Response.Redirect(
                    "~/Account/Login.aspx"
                );

                return;
            }


            if (!IsPostBack)
            {
                LoadReports();
            }
        }


        // =========================================================
        // CHECK MANAGER LOGIN
        // =========================================================

        private bool IsManagerLoggedIn()
        {
            if (!Request.IsAuthenticated)
            {
                return false;
            }


            if (Context.User != null &&
                Context.User.IsInRole("Manager"))
            {
                return true;
            }


            if (Session["UserRole"] != null)
            {
                string role =
                    Session["UserRole"]
                        .ToString()
                        .Trim();


                return role.Equals(
                    "Manager",
                    StringComparison.OrdinalIgnoreCase
                );
            }


            return false;
        }


        // =========================================================
        // LOAD EVERYTHING
        // =========================================================

        private void LoadReports()
        {
            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();


                    // Update old pending sessions first.
                    AutoCompletePastSessions(
                        connection
                    );


                    LoadMemberStatistics(
                        connection
                    );



                    LoadSessionStatistics(
                        connection
                    );


                    LoadTrainerActivity(
                        connection
                    );
                }
            }
            catch (Exception ex)
            {
                lblReportMessage.Text =
                    "The report could not be loaded: "
                    + ex.Message;
            }
        }


        // =========================================================
        // MEMBERS
        // =========================================================

        private void LoadMemberStatistics(
            SqlConnection connection)
        {
            const string sql = @"
                SELECT

                    COUNT(*) AS TotalMembers,

                    SUM(
                        CASE
                            WHEN LOWER(
                                LTRIM(
                                    RTRIM(
                                        ISNULL(
                                            Member_Status,
                                            ''
                                        )
                                    )
                                )
                            ) = 'active'
                            THEN 1
                            ELSE 0
                        END
                    ) AS ActiveMembers

                FROM Member;
            ";


            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection))
            {
                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int total =
                            Convert.ToInt32(
                                reader["TotalMembers"]
                            );


                        int active =
                            reader["ActiveMembers"]
                                == DBNull.Value
                                ? 0
                                : Convert.ToInt32(
                                    reader["ActiveMembers"]
                                );


                        int inactive =
                            total - active;


                        lblTotalMembers.Text =
                            total.ToString();


                        lblActiveMembers.Text =
                            active.ToString();


                        lblMembershipActive.Text =
                            active.ToString();


                        lblInactiveMembers.Text =
                            inactive.ToString();


                        lblMembershipTotal.Text =
                            total.ToString();
                    }
                }
            }


            // =====================================================
            // TRAINERS
            // =====================================================

            const string trainerSql = @"
                SELECT COUNT(*)
                FROM Staff
                WHERE Staff_Role = 'Trainer';
            ";


            using (SqlCommand command =
                   new SqlCommand(
                       trainerSql,
                       connection))
            {
                object result =
                    command.ExecuteScalar();


                int trainerCount =
                    result == null ||
                    result == DBNull.Value
                        ? 0
                        : Convert.ToInt32(result);


                lblTotalTrainers.Text =
                    trainerCount.ToString();
            }
        }


        // =========================================================
        // SESSION STATISTICS
        // =========================================================

        private void LoadSessionStatistics(
            SqlConnection connection)
        {
            const string sql = @"
                SELECT

                    COUNT(*) AS TotalSessions,

                    SUM(
                        CASE
                            WHEN LOWER(
                                LTRIM(
                                    RTRIM(
                                        ISNULL(
                                            Session_Status,
                                            'Pending'
                                        )
                                    )
                                )
                            ) = 'pending'
                            THEN 1
                            ELSE 0
                        END
                    ) AS PendingSessions,

                    SUM(
                        CASE
                            WHEN LOWER(
                                LTRIM(
                                    RTRIM(
                                        ISNULL(
                                            Session_Status,
                                            ''
                                        )
                                    )
                                )
                            ) = 'completed'
                            THEN 1
                            ELSE 0
                        END
                    ) AS CompletedSessions,

                    SUM(
                        CASE
                            WHEN LOWER(
                                LTRIM(
                                    RTRIM(
                                        ISNULL(
                                            Session_Status,
                                            ''
                                        )
                                    )
                                )
                            ) IN
                            (
                                'cancelled',
                                'canceled'
                            )
                            THEN 1
                            ELSE 0
                        END
                    ) AS CancelledSessions

                FROM Training_Session;
            ";


            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection))
            {
                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int total =
                            Convert.ToInt32(
                                reader["TotalSessions"]
                            );


                        int pending =
                            Convert.ToInt32(
                                reader["PendingSessions"]
                            );


                        int completed =
                            Convert.ToInt32(
                                reader["CompletedSessions"]
                            );


                        int cancelled =
                            Convert.ToInt32(
                                reader["CancelledSessions"]
                            );


                        lblTotalSessions.Text =
                            total.ToString();


                        lblPendingSessions.Text =
                            pending.ToString();


                        lblCompletedSessions.Text =
                            completed.ToString();


                        lblCancelledSessions.Text =
                            cancelled.ToString();
                    }
                }
            }
        }


        // =========================================================
        // TRAINER ACTIVITY
        // =========================================================

        private void LoadTrainerActivity(
            SqlConnection connection)
        {
            DataTable table =
                new DataTable();


            const string sql = @"
                SELECT

                    ST.Staff_FName
                    + ' '
                    + ST.Staff_LName
                    AS Trainer_Name,

                    COUNT(
                        DISTINCT T.Member_ID
                    ) AS MemberCount,

                    COUNT(
                        T.Training_ID
                    ) AS TotalSessions,

                    SUM(
                        CASE
                            WHEN LOWER(
                                LTRIM(
                                    RTRIM(
                                        ISNULL(
                                            T.Session_Status,
                                            ''
                                        )
                                    )
                                )
                            ) = 'completed'
                            THEN 1
                            ELSE 0
                        END
                    ) AS CompletedSessions,

                    SUM(
                        CASE
                            WHEN LOWER(
                                LTRIM(
                                    RTRIM(
                                        ISNULL(
                                            T.Session_Status,
                                            'Pending'
                                        )
                                    )
                                )
                            ) = 'pending'
                            THEN 1
                            ELSE 0
                        END
                    ) AS PendingSessions,

                    SUM(
                        CASE
                            WHEN LOWER(
                                LTRIM(
                                    RTRIM(
                                        ISNULL(
                                            T.Session_Status,
                                            ''
                                        )
                                    )
                                )
                            ) IN
                            (
                                'cancelled',
                                'canceled'
                            )
                            THEN 1
                            ELSE 0
                        END
                    ) AS CancelledSessions

                FROM Staff AS ST

                LEFT JOIN Training_Session AS T
                    ON ST.Staff_ID =
                       T.Staff_ID

                WHERE ST.Staff_Role =
                      'Trainer'

                GROUP BY

                    ST.Staff_ID,

                    ST.Staff_FName,

                    ST.Staff_LName

                ORDER BY

                    ST.Staff_LName ASC,

                    ST.Staff_FName ASC;
            ";


            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection))
            {
                using (SqlDataAdapter adapter =
                       new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }


            gvTrainerReport.DataSource =
                table;

            gvTrainerReport.DataBind();
        }


        // =========================================================
        // AUTOMATICALLY COMPLETE PASSED SESSIONS
        // =========================================================

        private void AutoCompletePastSessions(
            SqlConnection connection)
        {
            const string sql = @"
                UPDATE T

                SET T.Session_Status =
                    'Completed'

                FROM Training_Session AS T

                INNER JOIN Time_Slot AS TS
                    ON T.Time_ID =
                       TS.Time_ID

                WHERE LOWER(
                    LTRIM(
                        RTRIM(
                            ISNULL(
                                T.Session_Status,
                                'Pending'
                            )
                        )
                    )
                ) = 'pending'

                AND DATEADD(
                    SECOND,

                    DATEDIFF(
                        SECOND,
                        CAST(
                            '00:00:00'
                            AS time
                        ),
                        TS.End_Time
                    ),

                    CAST(
                        T.Session_Date
                        AS datetime
                    )
                ) < GETDATE();
            ";


            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}