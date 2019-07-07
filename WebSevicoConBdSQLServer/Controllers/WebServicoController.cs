using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace WebSevicoConBdSQLServer.Controllers
{
    [RoutePrefix("api/WebServico")]
    public class WebServicoController : ApiController
    {


        // GET: api/authors
        [AcceptVerbs("GET")]
        [Route("INI")]
        public string INI()
        {

            return "ok rodadno" ;
        }

        [AcceptVerbs("GET","POST")]
        [Route("RotJson")]
        public HttpResponseMessage RotJson()
        { 
            var resultado = new
            {
                Nome = "Linha de Código",
                URL = "www.linhadecodigo.com.br"
            };
              
            var jsonString = Json(resultado).ToString();
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response;
             
        }

        [AcceptVerbs("GET", "POST")]
        [Route("RotJson2")]
        public JsonResult<Item> RotJson2()
        {
              
            var jsonString = Json(new Item() { Id = 12, Name = "json rest ok" }); 
            return jsonString;

        }


        // GET api/authors/version
        [AcceptVerbs("GET")] 
        [Route("Version")] 
        public string Version()
        {
            return "Version 1.0.0";
        }



        // GET api/authors/version
        [AcceptVerbs("GET")]
        [Route("conn")]
        public string conn()
        {
            //Nomeservidor
            //DESKTOP-7AOP7KI\SQLEXPRESS
            //DESKTOP-7AOP7KI\Dayvson
            //ConnStringDb1

            string conectStatus = "erro";
            try
            {
                // Build connection string
                /*SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = @"DESKTOP-7AOP7KI\\SQLEXPRESS";  // update me
                builder.UserID = "solange";       // update me 
                builder.Password = "12345";      // update me
                builder.InitialCatalog = "TODOS";*/
                


               // string connectString = "Data Source=(DESKTOP-7AOP7KI" +"\\" + "SQLEXPRESS);User ID=solange;Password=12345;Initial Catalog=TODOS";

                //SqlConnectionStringBuilder builder =new SqlConnectionStringBuilder(connectString);

                // Connect to SQL
                Console.Write("Connecting to SQL Server ... ");
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnStringDb1"].ConnectionString))
                {
                    connection.Open();
                    conectStatus = "conectado con sucesso " + " Database = " + connection.Database;
                    Console.WriteLine("Done.");
                }
            }
            catch (SqlException e)
            {
                conectStatus = "erro";
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("All done. Press any key to finish...");


            return conectStatus;
        }


    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}