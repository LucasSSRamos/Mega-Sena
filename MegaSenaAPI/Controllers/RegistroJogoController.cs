using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaSenaAPI.Models;
using Newtonsoft.Json;

namespace MegaSenaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroJogoController : ControllerBase
    {
        private const string JogosFilePath = "jogosMega.json";

        // Método para registrar um jogo
        [HttpPost("RegistrarJogo")]
        public IActionResult RegistrarJogo([FromBody] RegistroJogoModel jogo)
        {
            // Obter a data atual
            jogo.DataJogo = DateTime.Now;

            if (jogo == null)
            {
                return BadRequest("Dados do jogo inválidos.");
            }

            if (string.IsNullOrEmpty(jogo.Nome) || jogo.Nome.Length < 3 || jogo.Nome.Length > 255)
            {
                return BadRequest("Nome é obrigatório e deve ter entre 3 e 255 caracteres.");
            }

            // Validar o CPF usando a classe CPFValidator
            if (!CPFValidator.IsValid(jogo.Cpf))
            {
                return BadRequest("CPF inválido.");
            }

            // Ler jogos existentes
            var jogos = LerJogos();

            // Adicionar o novo jogo à lista
            jogos.Add(jogo);

            // Salvar no arquivo JSON
            SalvarJogos(jogos);

            return Ok("Jogo Registrado com sucesso!");
        }

        // Método para obter todos os jogos
        [HttpGet("ObterTodosOsJogos")]
        public IActionResult ObterTodosOsJogos()
        {
            var jogos = LerJogos();
            return Ok(jogos);
        }

        private List<RegistroJogoModel> LerJogos()
        {
            if (System.IO.File.Exists(JogosFilePath))
            {
                var json = System.IO.File.ReadAllText(JogosFilePath);
                return JsonConvert.DeserializeObject<List<RegistroJogoModel>>(json);
            }
            else
            {
                return new List<RegistroJogoModel>();
            }
        }

        private void SalvarJogos(List<RegistroJogoModel> jogos)
        {
            var json = JsonConvert.SerializeObject(jogos);
            System.IO.File.WriteAllText(JogosFilePath, json);
        }
    }
}