using CursoEFCore.Data;
using CursoEFCore.Domain;
using CursoEFCore.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CursoEFCore
{
    public class Program
    {
        static void Main(string[] args)
        {
            ConsultandoDadosOrderBy();

            Console.ReadKey();
        }

        private static void RemoverRegistro()
        {
            using var db = new ApplicationContext();

            // var cliente = db.Clientes.Find(2);

            // db.Clientes.Remove(cliente);
            // db.Remove(cliente);
            // db.Entry(cliente).State = EntityState.Deleted;

            var cliente = new Cliente
            {
                Id = 1003
            };

            db.Entry(cliente).State = EntityState.Deleted;

            db.SaveChanges();
        }

        private static void AtualizarDados()
        {
            using var db = new ApplicationContext();

            //var cliente = db.Clientes.Find(1);

            var cliente = new Cliente
            {
                Id = 1
            };

            var clienteDesconectado = new
            {
                Nome = "Cliente Desconectado",
                Telefone = "7966669999"
            };

            cliente.Nome = "Cliente Alterado Passo 1";

            // db.Entry(cliente).State = EntityState.Modified; -> Força a atualização de todas as propriedades

            db.Attach(cliente);
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);

            // db.Clientes.Update(cliente);
            var linhas = db.SaveChanges();

            Console.WriteLine(linhas);
        }

        private static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new ApplicationContext();
            var pedidos = db.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(p => p.Produto)
                .ToList();

            Console.WriteLine(pedidos.Count);
        }

        private static void CadastrarPedido()
        {
            using var db = new ApplicationContext();

            var clienteId = db.Clientes.Select(c => c.Id).FirstOrDefault();
            var produtoId = db.Produtos.Select(p => p.Id).FirstOrDefault();

            var pedido = new Pedido
            {
                ClienteId = clienteId,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido Teste",
                Status = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produtoId,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10
                    }
                }
            };

            db.Pedidos.Add(pedido);
            var registros = db.SaveChanges();

            Console.Write($"Total Registro(s): {registros}");
        }

        private static void ConsultandoDadosOrderBy()
        {
            using var db = new ApplicationContext();

            var consultaPorMetodo = db.Clientes
                .Where(c => c.Id > 0)
                .OrderBy(c => c.Id)
                .ToList();

            foreach (var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Cliente: {cliente.Id}");
            }
        }

        private static void ConsultandoDadosFirstOrDefault()
        {
            // FirstOrDefault() diferente do Find() realiza a query novamente no banco de dados

            using var db = new ApplicationContext();

            var consultaPorMetodo = db.Clientes.Where(c => c.Id > 0).ToList();

            foreach (var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando Cliente: {cliente.Id}");
                db.Clientes.FirstOrDefault(c => c.Id == cliente.Id);
            }
        }

        private static void ConsultandoDadosFind()
        {
            // Método Find verifica se o registro já foi carregado em memória
            // Caso esteja em memória não faz uma nova query no banco
            // Com a chamada do método AsNoTracking() não é mantido o rastreio na memória
            // E ao chamar o método Find() uma nova query é executada

            using var db = new ApplicationContext();

            var consultaPorMetodo = db.Clientes.AsNoTracking().Where(c => c.Id > 0).ToList();

            foreach (var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando Cliente: {cliente.Id}");
                db.Clientes.Find(cliente.Id);
            }
        }

        private static void ConsultandoDadosPorMetodo()
        {
            using var db = new ApplicationContext();

            var consultaPorMetodo = db.Clientes.Where(c => c.Id > 0).ToList();

            foreach (var cliente in consultaPorMetodo)
            {
                Console.WriteLine(cliente.Nome);
            }
        }

        private static void ConsultandoDadosPorSintaxe()
        {
            using var db = new ApplicationContext();

            var consultaPorSintaxe = (from c in db.Clientes where c.Id > 0 select c).ToList();

            foreach (var cliente in consultaPorSintaxe)
            {
                Console.WriteLine(cliente.Nome);
            }
        }

        private static void InserindoRegistrosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Rafael Almeida",
                CEP = "99999000",
                Cidade = "Itabaiana",
                Estado = "SE",
                Telefone = "99000001111"
            };

            using var db = new ApplicationContext();

            db.AddRange(produto, cliente);

            var registros = db.SaveChanges();

            Console.Write($"Total Registro(s): {registros}");
        }

        private static void InserindoRegistros()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            using var db = new ApplicationContext();

            db.Produtos.Add(produto);
            db.Set<Produto>().Add(produto);
            db.Entry(produto).State = EntityState.Added;

            db.Add(produto);

            var registros = db.SaveChanges();

            Console.WriteLine(registros);
        }
    }
}
