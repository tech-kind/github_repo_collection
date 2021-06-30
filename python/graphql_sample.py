#!/usr/bin/env python3

from os import getenv
from request_module import BaseRequest
import pprint

def get_repo_info(url: str, access_token: str) -> dict:
    result_data: dict = {}
    graphql_client = BaseRequest(url, access_token)

    query = { 'query' : """
        query { 
            user(login: "tech-kind") {
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
        }
        """
    }

    graphql_client.post(query)

    if not graphql_client.check_status():
        print("response error!!")
        return result_data

    result_data = graphql_client.get_json_data()["data"]["user"]
    return result_data

def print_result_data(data: dict):
    print("User URL = {}".format(data["url"]))
    print("Repositories Count = {}".format(data["repositories"]["totalCount"]))
    for node in data["repositories"]["nodes"]:
        print("--------------------")
        print("Repo Name = {}".format(node["name"]))
        print("Repo URL = {}".format(node["url"]))
        print("Repo Description = {}".format(node["description"]))
        print("Repo CreatedAt = {}".format(node["createdAt"]))
        print("Repo UpdatedAt = {}".format(node["updatedAt"]))


if __name__ == '__main__':
    url: str = "https://api.github.com/graphql"
    access_token: str = getenv("GitHubKey")
    result_data: dict = get_repo_info(url, access_token)
    if len(result_data) != 0:
        print_result_data(result_data)
