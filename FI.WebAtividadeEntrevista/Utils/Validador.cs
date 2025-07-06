using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FI.AtividadeEntrevista.DML;
using Microsoft.Ajax.Utilities;

namespace FI.WebAtividadeEntrevista.Utils
{
    public static class Validador
    {
        public static bool VeficaBeneficiorioIgualCliente(List<Beneficiario> beneficiarios, string cpfCliente)
        {
            if (beneficiarios.Any(x=>x.CPF == cpfCliente))
            {
                return false;
            }
            return true;
        }
        public static bool VerificaCPFsDuplicadosBeneficiarios(List<Beneficiario> beneficiarios)
        {
            var beneficiariosDistintos = beneficiarios.DistinctBy(x => x.CPF).ToList();
            if (beneficiarios.Count > beneficiariosDistintos.Count)
            {
                return false;

            }
            return true;

        }
        public static bool ValidaCPF(string cpf)
        {

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = Desmascara(cpf);
            if (cpf.Length != 11)
            {
                return false;
            }
            if (cpf.All(digit=> digit == cpf[0]))
            {
                return false;
            }

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            }

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito = resto.ToString();
            tempCpf += digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            }

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }
            public static string Desmascara(string texto)
        {
            return string.IsNullOrEmpty(texto)
                ? texto
                : texto.ToString().Replace("-", string.Empty).Replace("_", string.Empty).Replace("/", string.Empty).Replace("\\", string.Empty).Replace(".", string.Empty);
        }
    }
}