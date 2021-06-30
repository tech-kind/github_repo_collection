using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace graphql_sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // アクセストークンは環境変数から読み込む
            var key = Environment.GetEnvironmentVariable("GitHubKey", EnvironmentVariableTarget.User);

            // GitHubのGraphQLのエンドポイントの指定
            var graphQLClient = new GraphQLHttpClient("https://api.github.com/graphql", new NewtonsoftJsonSerializer());
            // アクセストークンをリクエストのヘッダに追加
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {key}");

            // クエリに取得したい情報を記述する
            // どのような結果が取得できるのかは予め以下のURLで確認しておくとよい
            // https://docs.github.com/en/graphql/overview/explorer
            var repositoriesRequest = new GraphQLRequest
            {
                Query = @"
                query { 
                    user(login: ""tech-kind"") {
                        name
                        url
                        repositories(last: 20) {
                            totalCount
                            nodes {
                                name
                                description
                                createdAt
                                updatedAt
                                url
                            }
                        }
                    }
                }"
            };

            var graphQLResponse = await graphQLClient.SendQueryAsync<ResponseType>(repositoriesRequest);

            // ----- 取得した結果の出力 -----
            Console.WriteLine($"User URL = {graphQLResponse.Data.User.Url}");
            Console.WriteLine($"Repositories Count = {graphQLResponse.Data.User.Repositories.TotalCount}");

            foreach(var node in graphQLResponse.Data.User.Repositories.Nodes)
            {
                Console.WriteLine("--------------------");
                Console.WriteLine($"Repo Name = {node.Name}");
                Console.WriteLine($"Repo URL = {node.Url}");
                Console.WriteLine($"Repo Description = {node.Description}");
                Console.WriteLine($"Repo CreatedAt = {node.CreatedAt}");
                Console.WriteLine($"Repo UpdatedAt = {node.UpdatedAt}");
            }
            // ------------------------------
        }

        #region GraphQLからのレスポンスを受けるためのクラス
        public class ResponseType
        {
            public UserType User { get; set; }
        }

        public class UserType
        {
            /// <summary>
            /// ユーザー名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// ユーザーのGitHub URL
            /// </summary>
            public string Url { get; set; }

            public RepositoryType Repositories { get; set; }
        }

        public class RepositoryType
        {
            /// <summary>
            /// 取得したリポジトリの総数
            /// </summary>
            public string TotalCount { get; set; }

            /// <summary>
            /// リポジトリの情報リスト
            /// </summary>
            public List<NodeType> Nodes { get; set; }
        }

        public class NodeType
        {
            /// <summary>
            /// リポジトリ名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// リポジトリの説明
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// 作成日時
            /// </summary>
            public string CreatedAt { get; set; }

            /// <summary>
            /// 更新日時
            /// </summary>
            public string UpdatedAt { get; set; }

            /// <summary>
            /// リポジトリURL
            /// </summary>
            public string Url { get; set; }
        }
        #endregion
    }
}
