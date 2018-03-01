
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<string> list = new List<string>();
//            JObject.Parse("{\"state\": \"open\","+
//    "\"settings\": {" +
//        "\"index\": {" +
//            "\"creation_date\": \"1515487033901\"," +
//            "\"number_of_shards\": \"1\"," +
//            "\"number_of_replicas\": \"1\"," +
//            "\"uuid\": \"lPHJY3wLRlOhYLj48IGObA\"," +
//            "\"version\": {" +
//                \"created\": \"6010199\"" +
//            },"+
//            \"provided_name\": \".kibana\""+
//        }"+
//},"+
//    \"mappings\": {"+
//        \"doc\": {"+
//            \"dynamic\": \"strict\","+
//            \"properties\": {"+
//                \"index-pattern\": {"+
//                    \"properties\": {"+
//                        \"notExpandable\": {"+
//                            \"type\": \"boolean\""+
//                        },"+
//                        \"fieldFormatMap\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"sourceFilters\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"timeFieldName\": {"+
//                            \"type\": \"keyword\""+
//                        },"+
//                        \"intervalName\": {"+
//                            \"type\": \"keyword\""+
//                        },"+
//                        \"fields\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"title\": {"+
//                            \"type\": \"text\""+
//                        }"+
//                    }"+
//                },"+
//                \"server\": {"+
//                    \"properties\": {"+
//                        \"uuid\": {"+
//                            \"type\": \"keyword\""+
//                        }"+
//                    }"+
//                },"+
//                \"search\": {"+
//                    \"properties\": {"+
//                        \"hits\": {"+
//                            \"type\": \"integer\""+
//                        },"+
//                        \"columns\": {"+
//                            \"type\": \"keyword\""+
//                        },"+
//                        \"description\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"sort\": {"+
//                            \"type\": \"keyword\""+
//                        },"+
//                        \"title\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"version\": {"+
//                            \"type\": \"integer\""+
//                        },"+
//                        \"kibanaSavedObjectMeta\": {"+
//                            \"properties\": {"+
//                                \"searchSourceJSON\": {"+
//                                    \"type\": \"text\""+
//                                }"+
//                            }"+
//                        }"+
//                    }"+
//                },"+
//                \"visualization\": {"+
//                    \"properties\": {"+
//                        \"savedSearchId\": {"+
//                            \"type\": \"keyword\""+
//                        },"+
//                        \"description\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"uiStateJSON\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"title\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"version\": {"+
//                            \"type\": \"integer\""+
//                        },"+
//                        \"kibanaSavedObjectMeta\": {"+
//                            \"properties\": {"+
//                                \"searchSourceJSON\": {"+
//                                    \"type\": \"text\""+
//                                }"+
//                            }"+
//                        },"+
//                        \"visState\": {"+
//                            \"type\": \"text\""+
//                        }"+
//                    }"+
//                },"+
//                \"graph-workspace\": {"+
//                    \"properties\": {"+
//                        \"numVertices\": {"+
//                            \"type\": \"integer\""+
//                        },"+
//                        \"description\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"numLinks\": {"+
//                            \"type\": \"integer\""+
//                        },"+
//                        \"title\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"version\": {"+
//                            \"type\": \"integer\""+
//                        },"+
//                        \"kibanaSavedObjectMeta\": {"+
//                            \"properties\": {"+
//                                \"searchSourceJSON\": {"+
//                                    \"type\": \"text\""+
//                                }"+
//                            }"+
//                        },"+
//                        \"wsState\": {"+
//                            \"type\": \"text\""+
//                        }"+
//                    }"+
//                },"+
//                \"updated_at\": {"+
//                    \"type\": \"date\""+
//                },"+
//                \"timelion-sheet\": {"+
//                    \"properties\": {"+
//                        \"hits\": {"+
//                            \"type\": \"integer\""+
//                        },"+
//                        \"timelion_sheet\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"timelion_interval\": {"+
//                            \"type\": \"keyword\""+
//                        },"+
//                        \"timelion_columns\": {"+
//                            \"type\": \"integer\""+
//                        },"+
//                        \"timelion_other_interval\": {"+
//                            \"type\": \"keyword\""+
//                        },"+
//                        \"timelion_rows\": {"+
//                            \"type\": \"integer\""+
//                        },"+
//                        \"description\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"title\": {"+
//                            \"type\": \"text\""+
//                        },"+
//                        \"version\": {"+
//                            \"type\": \"integer\""+
//                        },"+
//                        \"kibanaSavedObjectMeta\": {"+
//                            \"properties\": {"+
//                                \"searchSourceJSON\": {"+
//                                    \"type\": \"text\""+
//                                }"+
//                            }"+
//                        },"+
//                        \"timelion_chart_height\": {"+
//                            \"type\": \"integer\""+
//                        }"+
//                    }"+
//                },"+
//                \"type\": {"+
//                    \"type\": \"keyword\""+
//                },"+
//                \"config\": {"+
//                    \"dynamic\": \"true\","+
//                    \"properties\": {"+
//                        \"buildNum\": {"+
//                            \"type\": \"keyword\""+
//                        },"+
//                        \"defaultIndex\": {"+
//                            \"type\": \"text\","+
//                            \"fields\": {"+
//                                \"keyword\": {"+
//                                    \"ignore_above\": 256,"+
//                                    \"type\": \"keyword\""+
//                                }"+
//                            }"+
//                        },"+
//                        \"xPackMonitoring:showBanner\": {"+
//                            \"type\": \"boolean\""+
//                        }"+
//                    }"+
//                },"+
//                \"dashboard\": {"+
//                    \"properties\": {"+
//                        \"hits\": {"+
//                            \"type\": \"integer\""+
//                        },
//                        \"timeFrom\": {
//                            \"type\": \"keyword\"
//                        },
//                        \"timeTo\": {
//                            \"type\": \"keyword\"
//                        },
//                        \"refreshInterval\": {
//                            \"properties\": {
//                                \"display\": {
//                                    \"type\": \"keyword\"
//                                },
//                                \"section\": {
//                                    \"type\": \"integer\"
//                                },
//                                \"value\": {
//                                    \"type\": \"integer\"
//                                },
//                                \"pause\": {
//                                    \"type\": \"boolean\"
//                                }
//                            }
//                        },
//                        \"description\": {
//                            \"type\": \"text\"
//                        },
//                        \"uiStateJSON\": {
//                            \"type\": \"text\"
//                        },
//                        \"timeRestore\": {
//                            \"type\": \"boolean\"
//                        },
//                        \"title\": {
//                            \"type\": \"text\"
//                        },
//                        \"version\": {
//                            \"type\": \"integer\"
//                        },
//                        \"kibanaSavedObjectMeta\": {
//                            \"properties\": {
//                                \"searchSourceJSON\": {
//                                    \"type\": \"text\"
//                                }
//                            }
//                        },
//                        \"optionsJSON\": {
//                            \"type\": \"text\"
//                        },
//                        \"panelsJSON\": {
//                            \"type\": \"text\"
//                        }
//                    }
//                },
//                \"url\": {
//                    \"properties\": {
//                        \"accessCount\": {
//                            \"type\": \"long\"
//                        },
//                        \"accessDate\": {
//                            \"type\": \"date\"
//                        },
//                        \"url\": {
//                            \"type\": \"text\",
//                            \"fields\": {
//                                \"keyword\": {
//                                    \"ignore_above\": 2048,
//                                    \"type\": \"keyword\"
//                                }
//                            }
//                        },
//                        \"createDate\": {
//                            \"type\": \"date\"
//                        }
//                    }
//                }
//            }
//        }
//    },
//    \"aliases\": [ ],
//    \"primary_terms\": {
//        \"0\": 8
//    },
//    \"in_sync_allocations\": {
//        \"0\": [
//            \"NTssXmAuRt20Z0gMIGbRwg\"
//        ]
//    }

//}");
//            esHelper.Common.EsService.GetFieldsByJson()
        }


        public static void GetFieldsByJson(JObject jsonObj, List<string> list)
        {
            foreach (JProperty jpro in jsonObj.Properties())
            {
                if (jpro.Value is JValue)
                {
                    //string aa = jpro.Path + jpro.Name;
                    list.Add(jpro.Name);
                }
                else if (jpro.Value is JObject)
                {
                    GetFieldsByJson((JObject)jpro.Value, list);
                }
            }
        }
    }
}
