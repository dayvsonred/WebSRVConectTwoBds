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
using System.Collections;
using System.Threading.Tasks;

namespace WebSevicoConBdSQLServer.Controllers
{
    [RoutePrefix("api/WebServico")]
    public class WebServicoController : ApiController
    {


        private static bool Open = true;

        private string  WebAtualzado;
         
        // GET: api/authors
        [AcceptVerbs("GET")]
        [Route("INI")]
        public string INI()
        {
            Open = true; 
            WebAtualzado = "";
            return "ok rodadno" ;
        }

        // GET: api/authors
        [AcceptVerbs("GET")]
        [Route("FIM")]
        public string FIM()
        {
            Open = false;
            return "parado rodadno" + Open.ToString();
        }


        // GET: api/authors
        [AcceptVerbs("GET")]
        [Route("PG")]
        public string PG()
        {
            return WebAtualzado;
        }

        // GET: api/authors
        [AcceptVerbs("GET")]
        [Route("PGOPEN")]
        public async Task <IHttpActionResult> PGOPEN()
        {

            var httpContext = Request.Properties["MS_HttpContext"] as HttpContextBase;
            //if (Open)
                //await EnviarNoticias();
               

            return Ok<string>(WebAtualzado);
        }

        // GET: api/authors
        [AcceptVerbs("GET")]
        [Route("PGOPEN2")]
        public string PGOPEN2()
        {

            //var httpContext = Request.Properties["MS_HttpContext"] as HttpContextBase;
            //if (Open)
            EnviarNoticias();

            return "exe ini ok = " +  WebAtualzado;
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
                    connection.Close(); 
                }
            }
            catch (SqlException e)
            {
                conectStatus = "erro";
                Console.WriteLine(e.ToString());
            }
              
            return conectStatus;
        }


        // GET  
        [AcceptVerbs("GET")]
        [Route("Selec01")]
        public JsonResult<IList> Selec01()
        {

            IList resultado = new ArrayList();

            string conectStatus = "erro";
            try
            {
               
                string sql = null;
                sql = "select * from TODOS.dbo.REGISTROS;";
                SqlCommand command;
                SqlDataReader dataReader;

                //var BD_Registros = new TBL_Regitros();
               /// var RegintroString = new TBL_Regitros() { Id = 12, Name = "json rest ok", Car = "" };
               
               // resultado.Add(RegintroString);



                // Connect to SQL
                Console.Write("Connecting to SQL Server ... ");
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnStringDb1"].ConnectionString))
                {
                    connection.Open();
                    conectStatus = "conectado con sucesso " + " Database = " + connection.Database;
                    Console.WriteLine("Done.");

                    command = new SqlCommand(sql, connection);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    { 
                        System.Diagnostics.Debug.WriteLine(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + " - " + dataReader.GetValue(2));
                        DateTime thisDay = DateTime.Now;

                        var itemID = dataReader.GetValue(0).ToString();
                        var itemNoome = dataReader.GetValue(1).ToString();
                        var itemCarr = dataReader.GetValue(2).ToString();
                        var agora = thisDay.ToString();
                        var RegintroString = new TBL_Regitros() { Id = Convert.ToInt32(itemID), Name = itemNoome, Car = itemCarr, Agora = agora};
                        resultado.Add(RegintroString);

                    }
                    dataReader.Close();
                    command.Dispose();
                    connection.Close();


                }
            }
            catch (SqlException e)
            {
                conectStatus = "erro";
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("All done. Press any key to finish...");


            var jsonString = Json(resultado);
            return jsonString;
        }



        // GET  
        [AcceptVerbs("GET")]
        [Route("STATRT_PRG")]
        public async Task<JsonResult<IList>> STATRT_PRG()
        {

            IList resultado = new ArrayList();

            int NOVOITEM = await ExiteNovo();
            //dynamic jsonRetorno = Json("STATRT_PRG json rest  = " + NOVOITEM);
            
            if (NOVOITEM > 0)
            {
                 IList ITEMS = await GetItensNovos();
                 
                foreach (TBL_Regitros element in ITEMS)
                { 
                    System.Diagnostics.Debug.WriteLine(element.Name.ToString() );
                    resultado.Add(element);
                    //insere no bd
                    int itemInsert = await SetItens(element.Id, element.Name.ToString(),element.Car.ToString(),element.Name.ToString(),"v");
                    //seinsert update
                    if (itemInsert > 0)
                    {
                        // UpdateItems(element.Id);
                    }
                }

                
                return Json(resultado);
                
            }
            else
            {
                var jsonRes = new MSG() { mensagem = "json rest ok" };
                resultado.Add(jsonRes);
                return Json(resultado);
            }
             
            
           
        }


        //conta numero de intens novos
        private async Task<int> ExiteNovo(){ 
            int retorno = 0; 
            try
            { 
                string sql = null;
                sql = "select  COUNT(*) as NUM from TODOS.dbo.REGISTROS where status = 'nv';";
                SqlCommand command;
                SqlDataReader dataReader; 
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnStringDb1"].ConnectionString))
                {
                    connection.Open();   
                    command = new SqlCommand(sql, connection);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        //System.Diagnostics.Debug.WriteLine(dataReader.GetValue(0));
                        DateTime thisDay = DateTime.Now;
                        retorno = Convert.ToInt32(dataReader.GetValue(0));
                    }
                    dataReader.Close();
                    command.Dispose();
                    connection.Close();
                }
            }
            catch (SqlException e)
            { 
                Console.WriteLine(e.ToString());
            } 
            return retorno;  
        }


        private async Task<IList> GetItensNovos()
        {
            ArrayList resultado = new ArrayList();
            try
            {
                string sql = null;
                sql = "select  * from TODOS.dbo.REGISTROS where status = 'nv';";
                SqlCommand command;
                SqlDataReader dataReader;
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnStringDb1"].ConnectionString))
                {
                    connection.Open();
                    command = new SqlCommand(sql, connection);
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine(dataReader.GetValue(0));
                        DateTime thisDay = DateTime.Now; 

                        var itemID = dataReader.GetValue(0).ToString();
                        var itemNoome = dataReader.GetValue(1).ToString();
                        var itemCarr = dataReader.GetValue(2).ToString();
                        var agora = thisDay.ToString();
                        var RegintroString = new TBL_Regitros() { Id = Convert.ToInt32(itemID), Name = itemNoome, Car = itemCarr, Agora = agora };
                        resultado.Add(RegintroString);
                    }
                    dataReader.Close();
                    command.Dispose();
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            //var jsonRes = Json(resultado);
            return resultado;
        }

        private async Task<int> SetItens(int val00, string val01, string val02,string val03, string val04)
        {
            int resultado = 0;
            try
            {
                string sql = null;
                sql = @" INSERT INTO[dbo].[REGISTROS] VALUES (" + val00 + "  ,'" + val01 + "' ,'" + val02 + "' ,'" + val03 + "' ,'" + val04 + "')";
                SqlCommand command; 
                using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnStringDb1"].ConnectionString))
                {
                    connection.Open();
                    command = new SqlCommand(sql, connection);
                    resultado = 0;
                    try {
                        resultado = command.ExecuteNonQuery();

                    } catch {
                        resultado = 0;
                    } 
                    command.Dispose();
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            } 
            return resultado;
        }



        private async Task EnviarNoticias()
        {
            WebAtualzado = "veio aq";

            while (true)
            {
                 Task.Delay(2000);
                DateTime thisDay = DateTime.Now;

                //if (Open)
                //{
                System.Diagnostics.Debug.WriteLine("---------------"); System.Diagnostics.Debug.WriteLine("---------------"); System.Diagnostics.Debug.WriteLine("---------------");
                System.Diagnostics.Debug.WriteLine(thisDay.ToString());

                SetTime(thisDay.ToString());


                // }
            }
        }




        private async Task SetTime(string timeString)
        {
             WebAtualzado = " PROG " + timeString;
            System.Diagnostics.Debug.WriteLine("++++++++++++++++++++++++++++++");
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TBL_Regitros
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Car { get; set; }
        public string Agora { get; set; }
    }

    public class ItemNovo
    {
        public int Num { get; set; } 
    }

    public class MSG
    {
        public string mensagem { get; set; }
    }


}