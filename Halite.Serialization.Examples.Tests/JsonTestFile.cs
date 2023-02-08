namespace Halite.Serialization.Examples.Tests;

using System;
using System.IO;
using System.Reflection;

public static class JsonTestFile
{
    public const string OrdersEmbedded =
        """
        {
            "ea:order": [
                {
                "_links": {
                    "self": {
                    "href": "/orders/123"
                    },
                    "ea:basket": {
                    "href": "/baskets/98712"
                    },
                    "ea:customer": {
                    "href": "/customers/7809"
                    }
                },
                "total": 30.00,
                "currency": "USD",
                "status": "shipped"
                },
                {
                "_links": {
                    "self": {
                    "href": "/orders/124"
                    },
                    "ea:basket": {
                    "href": "/baskets/97213"
                    },
                    "ea:customer": {
                    "href": "/customers/12369"
                    }
                },
                "total": 20.00,
                "currency": "USD",
                "status": "processing"
                }
            ]
        }
        """;

    public const string OrdersResource =
        """
        {
            "_links": {
                "self": {
                "href": "/orders"
                },
                "curies": [
                {
                    "name": "ea",
                    "href": "http://example.com/docs/rels/{rel}",
                    "templated": true
                }
                ],
                "next": {
                "href": "/orders?page=2"
                },
                "ea:find": {
                "href": "/orders{?id}",
                "templated": true
                },
                "ea:admin": [
                {
                    "href": "/admins/2",
                    "title": "Fred"
                },
                {
                    "href": "/admins/5",
                    "title": "Kate"
                }
                ]
            },
            "currentlyProcessing": 14,
            "shippedToday": 20,
            "_embedded": {
                "ea:order": [
                {
                    "_links": {
                    "self": {
                        "href": "/orders/123"
                    },
                    "ea:basket": {
                        "href": "/baskets/98712"
                    },
                    "ea:customer": {
                        "href": "/customers/7809"
                    }
                    },
                    "total": 30.00,
                    "currency": "USD",
                    "status": "shipped"
                },
                {
                    "_links": {
                    "self": {
                        "href": "/orders/124"
                    },
                    "ea:basket": {
                        "href": "/baskets/97213"
                    },
                    "ea:customer": {
                        "href": "/customers/12369"
                    }
                    },
                    "total": 20.00,
                    "currency": "USD",
                    "status": "processing"
                }
                ]
            }
        }
        """;
}