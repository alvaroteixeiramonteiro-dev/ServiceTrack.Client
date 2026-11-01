using ServiceTrack.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ServiceTrack.Client.Services
{
    public static class ApiService
    {
        private static readonly HttpClient _httpClient;

        // O serviço da API é executado automaticamente antes da classe ser usada pela primeira vez.
        static ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7242/")
            };

            // Indica que o cliente aceita respostas em JSON.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                "application/json"));
        }

        // GET: api/Pedidos — obtém todos os pedidos.
        public static async Task<List<Pedido>> ObterPedidosAsync()
        {
            try
            {
                var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>(
                    "api/Pedidos");

                return pedidos ?? new List<Pedido>();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Erro de conexão com a API: {ex.Message}", ex);
            }
        }

        // POST: api/Pedidos — cria um novo pedido.
        public static async Task<bool> CriarPedidoAsync(Pedido novoPedido)
        {
            var resposta = await _httpClient.PostAsJsonAsync(
                "api/Pedidos", novoPedido);

            return resposta.IsSuccessStatusCode;
        }

        // PUT: api/Pedidos/{id} — atualiza um pedido existente.
        public static async Task<bool> AtualizarPedidoAsync(
            int id, Pedido pedidoAtualizado)
        {
            var resposta = await _httpClient.PutAsJsonAsync(
                $"api/Pedidos/{id}", pedidoAtualizado);

            return resposta.IsSuccessStatusCode;
        }

        // DELETE: api/Pedidos/{id} — elimina um pedido.
        public static async Task<bool> EliminarPedidoAsync(int id)
        {
            var resposta = await _httpClient.DeleteAsync($"api/Pedidos/{id}");

            return resposta.IsSuccessStatusCode;
        }
    }
}
