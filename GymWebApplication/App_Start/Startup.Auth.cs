using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

namespace GymWebApplication
{
    public partial class Startup
    {
        // =========================================================
        // CONFIGURE AUTHENTICATION
        // =========================================================

        public void ConfigureAuth(IAppBuilder app)
        {
            /*
             * We are NOT using ASP.NET Identity for authentication.
             *
             * Our application uses the following database tables:
             *
             *      Login
             *      Member
             *      Staff
             *
             * The Login.aspx.cs file checks the Login table directly.
             *
             * After a successful login, this cookie is created and
             * ASP.NET uses it to remember that the user is logged in.
             */


            // =====================================================
            // APPLICATION LOGIN COOKIE
            // =====================================================

            app.UseCookieAuthentication(
                new CookieAuthenticationOptions
                {
                    /*
                     * This name MUST match the authentication type
                     * used when creating the login cookie.
                     */

                    AuthenticationType =
                        "GymApplicationCookie",


                    /*
                     * If an unauthenticated user tries to access
                     * a protected page, send them to Login.aspx.
                     */

                    LoginPath =
                        new PathString(
                            "/Account/Login.aspx"
                        ),


                    /*
                     * Keep the user logged in for 8 hours.
                     */

                    ExpireTimeSpan =
                        TimeSpan.FromHours(8),


                    /*
                     * Automatically extend the cookie when the
                     * user is active.
                     */

                    SlidingExpiration = true,


                    /*
                     * Cookie settings.
                     */

                    CookieHttpOnly = true,


                    /*
                     * Cookie provider.
                     */

                    Provider =
                        new CookieAuthenticationProvider
                        {
                            OnApplyRedirect = context =>
                            {
                                /*
                                 * Redirect unauthenticated users
                                 * to the login page.
                                 */

                                context.Response.Redirect(
                                    context.RedirectUri
                                );
                            }
                        }
                }
            );
        }
    }
}