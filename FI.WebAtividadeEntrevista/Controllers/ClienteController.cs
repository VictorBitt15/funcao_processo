﻿using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using FI.WebAtividadeEntrevista.Utils;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            if (!Validador.ValidaCPF(model.CPF))
            {
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, "CPF do cliente Inválido, digite novamente."));
            }

            BoCliente bo = new BoCliente();
            if (bo.VerificarExistencia(model.CPF))
            {
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, "CPF já cadastrado."));
            }

            if (model.Beneficiarios == null)
            {
                model.Beneficiarios = new List<Beneficiario>();
            }
            if (!Validador.VeficaBeneficiorioIgualCliente(model.Beneficiarios, model.CPF))
            {
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, "CPF do beneficiário não pode ser igual ao CPF do cliente."));

            }
            if (!Validador.VerificaCPFsDuplicadosBeneficiarios(model.Beneficiarios))
            {
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, "Exitem beneficiários com CPFs duplicados na lista."));
            }
            foreach (Beneficiario beneficiario in model.Beneficiarios)
            {
                if (!Validador.ValidaCPF(beneficiario.CPF))
                {
                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, "CPF do beneficiário " + beneficiario.Nome + " é inválido, por favor digite novamente."));
                }
            }


            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                
                model.Id = bo.Incluir(new Cliente()
                {                    
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF,
                    Beneficiarios = model.Beneficiarios,
                });

           
                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            if (!Validador.ValidaCPF(model.CPF))
            {
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, "CPF do cliente Inválido, digite novamente."));
            }
            

            BoCliente bo = new BoCliente();
            if (!bo.Consultar(model.Id).CPF.Equals(model.CPF))
            {
                if (bo.VerificarExistencia(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, "CPF já cadastrado."));
                }
            }

            if (model.Beneficiarios == null)
            {
                model.Beneficiarios = new List<Beneficiario>();
            }
            if (!Validador.VeficaBeneficiorioIgualCliente(model.Beneficiarios, model.CPF))
            {
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, "CPF do beneficiário não pode ser igual ao CPF do cliente."));

            }

            if (!Validador.VerificaCPFsDuplicadosBeneficiarios(model.Beneficiarios))
            {
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, "Exitem beneficiários com CPFs duplicados na lista."));
            }
            foreach (Beneficiario beneficiario in model.Beneficiarios)
            {
                if (!Validador.ValidaCPF(beneficiario.CPF))
                {
                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, "CPF do beneficiário " + beneficiario.Nome + " é inválido, por favor digite novamente."));
                }
            }

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF,
                    Beneficiarios = model.Beneficiarios
                });
                               
                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF,
                    Beneficiarios = cliente.Beneficiarios
                };

            
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}