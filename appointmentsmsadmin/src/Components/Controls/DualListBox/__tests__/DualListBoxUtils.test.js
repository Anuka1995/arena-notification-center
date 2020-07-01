import {findParent, findNode, GroupExcludeReshIds} from './../DualListBoxUtils'

test('Find node', () => {
    expect(findNode(overview, 'aabaju+0000000000021').title).toBe(`KIR kveldspoliklinikk`);
   });

test("find Parent", () => {
  var node = findNode(overview, 'aabaju+0000000000021')
  var parent = findParent(overview, node)
  expect(parent.title).toBe("Sections")
})

test('group all elements to section', () =>{
  let unitGids = ["aabaju+0000000000021", "aabaju+0000001000003", "aabaju+0000001000005",
  "aabaju+0000001000198", "aabaju+0000001000200","aabaju+0000001000297","aabaju+0000001000299"]
  let parent = "depsecaabafa+0000000000022"
  let grouped = GroupExcludeReshIds(overview, unitGids)
  
  expect(grouped[0]).toBe(parent)

})

test(' 2 groups', () => {
  let unitGids = ["aabaju+0000000000021", "aabaju+0000001000003", "aabaju+0000001000005",
  "aabaju+0000001000198", "aabaju+0000001000200","aabaju+0000001000297","aabaju+0000001000299",
  "aabaea+0000000000011","aabaea+0000001000096","aabaea+0000001000099","aabaea+0000001000117",
  "aabaea+0000001000119"]
  let parent = "depsecaabafa+0000000000022"
  let grouped = GroupExcludeReshIds(overview, unitGids)

  expect(grouped.length).toBe(2)
})

test(' 1 group 2 individuals', () => {
  let unitGids = ["aabaju+0000000000021", "aabaju+0000001000003", "aabaju+0000001000005",
  "aabaju+0000001000198", "aabaju+0000001000200","aabaju+0000001000297","aabaju+0000001000299",
  "aabaea+0000000000011","aabaea+0000001000096"]
  let parent = "depsecaabafa+0000000000022"
  let grouped = GroupExcludeReshIds(overview, unitGids)

  expect(grouped.length).toBe(3)
})

test(' 2 group with opd', () => {
  let unitGids = ["aabaju+0000000000021", "aabaju+0000001000003", "aabaju+0000001000005",
  "aabaju+0000001000198", "aabaju+0000001000200","aabaju+0000001000297","aabaju+0000001000299",
  "aabaea+0000000000011","aabaea+0000001000096"]
  let parent = "depsecaabafa+0000000000022"
  let grouped = GroupExcludeReshIds(overview, unitGids)

  expect(grouped.length).toBe(3)
})



var overview = [
  {
        "parentNode": null,
        "type": null,
        "childNodes": [{
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Test avdeling",
                "id": "aabafa+0000000000001",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000000000022",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "KIR kveldspoliklinikk",
                                "id": "aabaju+0000000000021",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Urologi",
                                "id": "aabaju+0000001000003",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Ortopedisk kirurgi",
                                "id": "aabaju+0000001000005",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Stomipasienter",
                                "id": "aabaju+0000001000198",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Generell kirurgi/gastro",
                                "id": "aabaju+0000001000200",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Seksjon erstattet",
                                "id": "aabaju+0000001000297",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Seksjon ut av bruk",
                                "id": "aabaju+0000001000299",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000000000022",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000000000022",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Kirurgisk post 2",
                                "id": "aabahl+0000000000021",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Intensiv post 1",
                                "id": "aabahl+0000000000025",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Kirurgisk post 1",
                                "id": "aabahl+0000000000029",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Intensiv post 2",
                                "id": "aabahl+0000001000001",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Gammel Intensiv",
                                "id": "aabahl+0000001000213",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Ikke tekn. intensivpost",
                                "id": "aabahl+0000001000215",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Post erstattet",
                                "id": "aabahl+0000001000252",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Automatisk test",
                                "id": "aabahl+0000001000254",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Akuttposten KIR/MED/ØNH/ØYE",
                                "id": "aabahl+0000001000277",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Hotell",
                                "id": "aabahl+0000001000398",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000000000022",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000000000022",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depopdaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Skadelegevakt",
                                "id": "aabaea+0000000000011",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Kirurgisk pol 1H",
                                "id": "aabaea+0000001000096",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Kirurgisk pol 2H",
                                "id": "aabaea+0000001000099",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Kirurgisk pol A",
                                "id": "aabaea+0000001000117",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000000000022",
                                "type": null,
                                "childNodes": [],
                                "title": "Kirurgisk pol B",
                                "id": "aabaea+0000001000119",
                                "tag": null
                            }
                        ],
                        "title": "OPDs",
                        "id": "depopdaabafa+0000000000022",
                        "tag": null
                    }
                ],
                "title": "Kirurgisk avdeling",
                "id": "aabafa+0000000000022",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000000000025",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "MED dagpasienter",
                                "id": "aabaju+0000000000041",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Lungesykdommer",
                                "id": "aabaju+0000001000007",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "MED kveldspoliklinikk",
                                "id": "aabaju+0000001000019",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Hjertesykdommer",
                                "id": "aabaju+0000001000204",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Generell medisin",
                                "id": "aabaju+0000001000206",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Blodsykdommer",
                                "id": "aabaju+0000001000208",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Gastromedisin",
                                "id": "aabaju+0000001000327",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Legevakt",
                                "id": "aabaju+0000001000329",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000000000025",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000000000025",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Medisinsk post 1",
                                "id": "aabahl+0000000000023",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Intensiv post 1",
                                "id": "aabahl+0000000000025",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Medisinsk post 2",
                                "id": "aabahl+0000001000015",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Akuttposten KIR/MED/ØNH/ØYE",
                                "id": "aabahl+0000001000277",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Hotell",
                                "id": "aabahl+0000001000398",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000000000025",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000000000025",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depopdaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Medisinsk pol 1H",
                                "id": "aabaea+0000001000101",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Medisinsk pol 2H",
                                "id": "aabaea+0000001000103",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Medisinsk pol A",
                                "id": "aabaea+0000001000121",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Medisinsk pol B",
                                "id": "aabaea+0000001000123",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000000000025",
                                "type": null,
                                "childNodes": [],
                                "title": "Hjerte poliklinikk",
                                "id": "aabaea+0000001000133",
                                "tag": null
                            }
                        ],
                        "title": "OPDs",
                        "id": "depopdaabafa+0000000000025",
                        "tag": null
                    }
                ],
                "title": "Medisinsk avdeling",
                "id": "aabafa+0000000000025",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000000000027",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000000000027",
                                "type": null,
                                "childNodes": [],
                                "title": "PSY seksjon for rus",
                                "id": "aabaju+0000000000043",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000000000027",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000000000027",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000000000027",
                                "type": null,
                                "childNodes": [],
                                "title": "Psykiatrisk post 1",
                                "id": "aabahl+0000000000027",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000027",
                                "type": null,
                                "childNodes": [],
                                "title": "Psykiatrisk post 2",
                                "id": "aabahl+0000001000003",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000027",
                                "type": null,
                                "childNodes": [],
                                "title": "Psykiatrisk post 3",
                                "id": "aabahl+0000001000005",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000000000027",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000000000027",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depopdaabafa+0000000000027",
                                "type": null,
                                "childNodes": [],
                                "title": "Rus poliklinikken",
                                "id": "aabaea+0000001000113",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000000000027",
                                "type": null,
                                "childNodes": [],
                                "title": "Psykiatrisk poliklinikk 1",
                                "id": "aabaea+0000001000115",
                                "tag": null
                            }
                        ],
                        "title": "OPDs",
                        "id": "depopdaabafa+0000000000027",
                        "tag": null
                    }
                ],
                "title": "Psykiatrisk avdeling",
                "id": "aabafa+0000000000027",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000000000042",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depopdaabafa+0000000000042",
                                "type": null,
                                "childNodes": [],
                                "title": "Røntgen sentrallab",
                                "id": "aabaea+0000001000076",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000000000042",
                                "type": null,
                                "childNodes": [],
                                "title": "Røntgen mammografi",
                                "id": "aabaea+0000001000078",
                                "tag": null
                            }
                        ],
                        "title": "OPDs",
                        "id": "depopdaabafa+0000000000042",
                        "tag": null
                    }
                ],
                "title": "Radiologisk avdeling",
                "id": "aabafa+0000000000042",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000000000044",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000000000044",
                                "type": null,
                                "childNodes": [],
                                "title": "Øye post 1",
                                "id": "aabahl+0000000000031",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000000000044",
                                "type": null,
                                "childNodes": [],
                                "title": "Akuttposten KIR/MED/ØNH/ØYE",
                                "id": "aabahl+0000001000277",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000000000044",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000000000044",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depopdaabafa+0000000000044",
                                "type": null,
                                "childNodes": [],
                                "title": "Øye poliklinikk 1H",
                                "id": "aabaea+0000001000105",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000000000044",
                                "type": null,
                                "childNodes": [],
                                "title": "Øye poliklinikk 2H",
                                "id": "aabaea+0000001000109",
                                "tag": null
                            }
                        ],
                        "title": "OPDs",
                        "id": "depopdaabafa+0000000000044",
                        "tag": null
                    }
                ],
                "title": "Øye avdelinga",
                "id": "aabafa+0000000000044",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Uaktuell avdeling",
                "id": "aabafa+0000000000061",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000000000073",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000000000073",
                                "type": null,
                                "childNodes": [],
                                "title": "Regnskap 1",
                                "id": "aabaju+0000000000045",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000000073",
                                "type": null,
                                "childNodes": [],
                                "title": "Regnskap 2",
                                "id": "aabaju+0000000000047",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000000000073",
                        "tag": null
                    }
                ],
                "title": "Regnskapsavstemmingsavdeling",
                "id": "aabafa+0000000000073",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000000000097",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000000000097",
                                "type": null,
                                "childNodes": [],
                                "title": "Sentrallaboratoriet",
                                "id": "aabaju+0000001000041",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000000000097",
                        "tag": null
                    }
                ],
                "title": "Klinisk kjemisk lab.",
                "id": "aabafa+0000000000097",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000000999999",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000000999999",
                                "type": null,
                                "childNodes": [],
                                "title": "Pediatri",
                                "id": "aabaju+0000001000210",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000000999999",
                                "type": null,
                                "childNodes": [],
                                "title": "Neonatalogi",
                                "id": "aabaju+0000001000212",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000000999999",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000000999999",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000000999999",
                                "type": null,
                                "childNodes": [],
                                "title": "Pediatrisk post",
                                "id": "aabahl+0000001000239",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000000999999",
                        "tag": null
                    }
                ],
                "title": "Barneavdelinga",
                "id": "aabafa+0000000999999",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000003",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000003",
                                "type": null,
                                "childNodes": [],
                                "title": "Gynekologi",
                                "id": "aabaju+0000001000214",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000001000003",
                                "type": null,
                                "childNodes": [],
                                "title": "Obstetrikk",
                                "id": "aabaju+0000001000216",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000001000003",
                                "type": null,
                                "childNodes": [],
                                "title": "Friske nyfødde",
                                "id": "aabaju+0000001000218",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000003",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000003",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000003",
                                "type": null,
                                "childNodes": [],
                                "title": "Intensiv post 1",
                                "id": "aabahl+0000000000025",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000003",
                                "type": null,
                                "childNodes": [],
                                "title": "Fødepost",
                                "id": "aabahl+0000001000219",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000003",
                                "type": null,
                                "childNodes": [],
                                "title": "Barselpost",
                                "id": "aabahl+0000001000221",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000003",
                                "type": null,
                                "childNodes": [],
                                "title": "Hotell",
                                "id": "aabahl+0000001000398",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000003",
                        "tag": null
                    }
                ],
                "title": "Føde",
                "id": "aabafa+0000001000003",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000005",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000005",
                                "type": null,
                                "childNodes": [],
                                "title": "Psykiatrisk akuttpost 1",
                                "id": "aabahl+0000001000007",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000005",
                                "type": null,
                                "childNodes": [],
                                "title": "Psykiatrisk akuttpost 2",
                                "id": "aabahl+0000001000009",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000005",
                        "tag": null
                    }
                ],
                "title": "Psykiatrisk akuttbehandling",
                "id": "aabafa+0000001000005",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000039",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000039",
                                "type": null,
                                "childNodes": [],
                                "title": "Intensiv post 2",
                                "id": "aabahl+0000001000001",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000039",
                                "type": null,
                                "childNodes": [],
                                "title": "Beleggspost",
                                "id": "aabahl+0000001000037",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000039",
                                "type": null,
                                "childNodes": [],
                                "title": "Beleggspost",
                                "id": "aabahl+0000001000039",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000039",
                                "type": null,
                                "childNodes": [],
                                "title": "Beleggspost - teknisk",
                                "id": "aabahl+0000001000097",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000039",
                        "tag": null
                    }
                ],
                "title": "Belegg, kun for beleggstest!",
                "id": "aabafa+0000001000039",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000059",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000059",
                                "type": null,
                                "childNodes": [],
                                "title": "Kløveråsen",
                                "id": "aabaju+0000001000350",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000059",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000059",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000059",
                                "type": null,
                                "childNodes": [],
                                "title": "Barne og ungd psyk",
                                "id": "aabahl+0000001000059",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000059",
                        "tag": null
                    }
                ],
                "title": "Barne og ungdomspsyk",
                "id": "aabafa+0000001000059",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000061",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000061",
                                "type": null,
                                "childNodes": [],
                                "title": "Habiliterig",
                                "id": "aabahl+0000001000057",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000061",
                        "tag": null
                    }
                ],
                "title": "Habilitering",
                "id": "aabafa+0000001000061",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Klinisk kjemisk lab",
                "id": "aabafa+0000001000079",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000119",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000119",
                                "type": null,
                                "childNodes": [],
                                "title": "Beleggspost - teknisk",
                                "id": "aabahl+0000001000097",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000119",
                                "type": null,
                                "childNodes": [],
                                "title": "Beleggspost3",
                                "id": "aabahl+0000001000099",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000119",
                        "tag": null
                    }
                ],
                "title": "Belegg2, kun for beleggstest!",
                "id": "aabafa+0000001000119",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Anestesi",
                "id": "aabafa+0000001000159",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000161",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000161",
                                "type": null,
                                "childNodes": [],
                                "title": "Intensiv post 1",
                                "id": "aabahl+0000000000025",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000161",
                                "type": null,
                                "childNodes": [],
                                "title": "Intensiv post 2",
                                "id": "aabahl+0000001000001",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000161",
                        "tag": null
                    }
                ],
                "title": "Intensiv",
                "id": "aabafa+0000001000161",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000272",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000272",
                                "type": null,
                                "childNodes": [],
                                "title": "Øre-nese-halsseksjonen",
                                "id": "aabaju+0000001000170",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000001000272",
                                "type": null,
                                "childNodes": [],
                                "title": "Kjevekirurgisk seksjon",
                                "id": "aabaju+0000001000178",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000001000272",
                                "type": null,
                                "childNodes": [],
                                "title": "Søvnpasienter",
                                "id": "aabaju+0000001000202",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000272",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000272",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000272",
                                "type": null,
                                "childNodes": [],
                                "title": "Intensiv post 1",
                                "id": "aabahl+0000000000025",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000272",
                                "type": null,
                                "childNodes": [],
                                "title": "Øre-nese-hals 1",
                                "id": "aabahl+0000001000211",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000272",
                                "type": null,
                                "childNodes": [],
                                "title": "Øre-nese-hals 2",
                                "id": "aabahl+0000001000241",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000272",
                                "type": null,
                                "childNodes": [],
                                "title": "Automatisk test",
                                "id": "aabahl+0000001000254",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000272",
                                "type": null,
                                "childNodes": [],
                                "title": "Akuttposten KIR/MED/ØNH/ØYE",
                                "id": "aabahl+0000001000277",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000272",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000272",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depopdaabafa+0000001000272",
                                "type": null,
                                "childNodes": [],
                                "title": "Kjevekirurgisk poliklinikk",
                                "id": "aabaea+0000001000107",
                                "tag": null
                            }
                        ],
                        "title": "OPDs",
                        "id": "depopdaabafa+0000001000272",
                        "tag": null
                    }
                ],
                "title": "Øre-nese-hals avdelingen",
                "id": "aabafa+0000001000272",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000274",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000274",
                                "type": null,
                                "childNodes": [],
                                "title": "Fysioterapipoliklinikken",
                                "id": "aabaju+0000001000172",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000001000274",
                                "type": null,
                                "childNodes": [],
                                "title": "Fysioterapi - psykisk helse",
                                "id": "aabaju+0000001000174",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000274",
                        "tag": null
                    }
                ],
                "title": "Fysioterapiavdelingen",
                "id": "aabafa+0000001000274",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Gammel psykiatrisk",
                "id": "aabafa+0000001000276",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000278",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000278",
                                "type": null,
                                "childNodes": [],
                                "title": "Bakteriologisk seksjon",
                                "id": "aabaju+0000001000176",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000278",
                        "tag": null
                    }
                ],
                "title": "Mikrobiologisk avd",
                "id": "aabafa+0000001000278",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Radiologisk avd 2",
                "id": "aabafa+0000001000280",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000302",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000302",
                                "type": null,
                                "childNodes": [],
                                "title": "Seksuelt overførbare infeksjoner",
                                "id": "aabaju+0000001000240",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000001000302",
                                "type": null,
                                "childNodes": [],
                                "title": "Ikke sensitive hudsykdommer",
                                "id": "aabaju+0000001000260",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000302",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000302",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000302",
                                "type": null,
                                "childNodes": [],
                                "title": "Beleggspost",
                                "id": "aabahl+0000001000037",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000302",
                        "tag": null
                    }
                ],
                "title": "Hudavdeling",
                "id": "aabafa+0000001000302",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Filter 0",
                "id": "aabafa+0000001000305",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Filter 1",
                "id": "aabafa+0000001000307",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Filter 2",
                "id": "aabafa+0000001000309",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "KIR BT",
                "id": "aabafa+0000001000325",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000327",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000327",
                                "type": null,
                                "childNodes": [],
                                "title": "BHK 1A",
                                "id": "aabaju+0000001000263",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000001000327",
                                "type": null,
                                "childNodes": [],
                                "title": "BHK 1B",
                                "id": "aabaju+0000001000265",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000327",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000327",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000327",
                                "type": null,
                                "childNodes": [],
                                "title": "Behandlerkrav",
                                "id": "aabahl+0000001000244",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000327",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000327",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depopdaabafa+0000001000327",
                                "type": null,
                                "childNodes": [],
                                "title": "BHK lok1",
                                "id": "aabaea+0000001000153",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000001000327",
                                "type": null,
                                "childNodes": [],
                                "title": "BHK lok2",
                                "id": "aabaea+0000001000155",
                                "tag": null
                            }
                        ],
                        "title": "OPDs",
                        "id": "depopdaabafa+0000001000327",
                        "tag": null
                    }
                ],
                "title": "Behandlerkrav1",
                "id": "aabafa+0000001000327",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000329",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000329",
                                "type": null,
                                "childNodes": [],
                                "title": "BHK 2A",
                                "id": "aabaju+0000001000267",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000001000329",
                                "type": null,
                                "childNodes": [],
                                "title": "BHK 2B",
                                "id": "aabaju+0000001000269",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000329",
                        "tag": null
                    }
                ],
                "title": "Behandlerkrav2",
                "id": "aabafa+0000001000329",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Behandlerkrav3",
                "id": "aabafa+0000001000331",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000353",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000353",
                                "type": null,
                                "childNodes": [],
                                "title": "BHK4 - seksjon 1",
                                "id": "aabaju+0000001000291",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000353",
                        "tag": null
                    }
                ],
                "title": "Behandlerkrav4",
                "id": "aabafa+0000001000353",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000355",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000355",
                                "type": null,
                                "childNodes": [],
                                "title": "EDI-Broker Test Post",
                                "id": "aabahl+0000001000246",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000355",
                                "type": null,
                                "childNodes": [],
                                "title": "EDI-Broker Test Post Erstattet",
                                "id": "aabahl+0000001000248",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000355",
                        "tag": null
                    }
                ],
                "title": "EDI-Broker Test Avd Erstattet",
                "id": "aabafa+0000001000355",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000357",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000357",
                                "type": null,
                                "childNodes": [],
                                "title": "EDI-Broker Test Post",
                                "id": "aabahl+0000001000246",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000357",
                                "type": null,
                                "childNodes": [],
                                "title": "EDI-Broker Test Post Erstattet",
                                "id": "aabahl+0000001000248",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000357",
                        "tag": null
                    }
                ],
                "title": "EDI-Broker Test Avd",
                "id": "aabafa+0000001000357",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000359",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000359",
                                "type": null,
                                "childNodes": [],
                                "title": "Kirurgisk post 2",
                                "id": "aabahl+0000000000021",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000359",
                                "type": null,
                                "childNodes": [],
                                "title": "Intensiv post 1",
                                "id": "aabahl+0000000000025",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000359",
                                "type": null,
                                "childNodes": [],
                                "title": "Kirurgisk post 1",
                                "id": "aabahl+0000000000029",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000359",
                                "type": null,
                                "childNodes": [],
                                "title": "Intensiv post 2",
                                "id": "aabahl+0000001000001",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000359",
                        "tag": null
                    }
                ],
                "title": "KIR erstattet",
                "id": "aabafa+0000001000359",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Klinisk Testlab",
                "id": "aabafa+0000001000361",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000363",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000363",
                                "type": null,
                                "childNodes": [],
                                "title": "Health_IntegrasjonsTest",
                                "id": "aabaju+0000001000301",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000363",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000363",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000363",
                                "type": null,
                                "childNodes": [],
                                "title": "Post_Health_IntegrasjonsTest",
                                "id": "aabahl+0000001000297",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000363",
                        "tag": null
                    }
                ],
                "title": "Avd_Health_IntegrasjonsTest",
                "id": "aabafa+0000001000363",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000365",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000365",
                                "type": null,
                                "childNodes": [],
                                "title": "Post_Health_IntegrasjonsTest",
                                "id": "aabahl+0000001000297",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000365",
                        "tag": null
                    }
                ],
                "title": "AvdelingIkkeIBruk",
                "id": "aabafa+0000001000365",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000387",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000387",
                                "type": null,
                                "childNodes": [],
                                "title": "HL7ConnectorTestSection1",
                                "id": "aabaju+0000001000323",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000001000387",
                                "type": null,
                                "childNodes": [],
                                "title": "HL7ConnectorTestSection2",
                                "id": "aabaju+0000001000325",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000387",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000387",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000387",
                                "type": null,
                                "childNodes": [],
                                "title": "HL7ConnectorTestPost1",
                                "id": "aabahl+0000001000319",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000387",
                                "type": null,
                                "childNodes": [],
                                "title": "HL7ConnectorTestPost2",
                                "id": "aabahl+0000001000321",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000387",
                        "tag": null
                    }
                ],
                "title": "HL7ConnectorTestAvd",
                "id": "aabafa+0000001000387",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "HL7ConnectorTestAvd2",
                "id": "aabafa+0000001000389",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "LAB proxy",
                "id": "aabafa+0000001000393",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000401",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000401",
                                "type": null,
                                "childNodes": [],
                                "title": "Seksjon for test av triggere",
                                "id": "aabaju+0000001000356",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000401",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000401",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000401",
                                "type": null,
                                "childNodes": [],
                                "title": "Post for test av triggere",
                                "id": "aabahl+0000001000334",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000401",
                        "tag": null
                    }
                ],
                "title": "Avdeling for test av triggere",
                "id": "aabafa+0000001000401",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000403",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000403",
                                "type": null,
                                "childNodes": [],
                                "title": "Barnepoliklinikken",
                                "id": "aabaju+0000001000370",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000001000403",
                                "type": null,
                                "childNodes": [],
                                "title": "Barn - Lungepoliklinikken",
                                "id": "aabaju+0000001000372",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000403",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000403",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depwardaabafa+0000001000403",
                                "type": null,
                                "childNodes": [],
                                "title": "Akutt - Barn",
                                "id": "aabahl+0000001000358",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000403",
                                "type": null,
                                "childNodes": [],
                                "title": "Barn - post 1",
                                "id": "aabahl+0000001000360",
                                "tag": null
                            }, {
                                "parentNode": "depwardaabafa+0000001000403",
                                "type": null,
                                "childNodes": [],
                                "title": "Barn - post 2",
                                "id": "aabahl+0000001000362",
                                "tag": null
                            }
                        ],
                        "title": "Wards",
                        "id": "depwardaabafa+0000001000403",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000403",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depopdaabafa+0000001000403",
                                "type": null,
                                "childNodes": [],
                                "title": "Barn og ungdom - 1. etg",
                                "id": "aabaea+0000001000186",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000001000403",
                                "type": null,
                                "childNodes": [],
                                "title": "Barn og ungdom - 2. etg",
                                "id": "aabaea+0000001000188",
                                "tag": null
                            }, {
                                "parentNode": "depopdaabafa+0000001000403",
                                "type": null,
                                "childNodes": [],
                                "title": "Barn og ungdom - 3. etg",
                                "id": "aabaea+0000001000190",
                                "tag": null
                            }
                        ],
                        "title": "OPDs",
                        "id": "depopdaabafa+0000001000403",
                        "tag": null
                    }
                ],
                "title": "Barne og ungdomsavdelingen",
                "id": "aabafa+0000001000403",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "Gastroenterologisk laboratorium",
                "id": "aabafa+0000001000423",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [],
                "title": "SchedulingSuite KIR",
                "id": "aabafa+0000001000523",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000545",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000545",
                                "type": null,
                                "childNodes": [],
                                "title": "XORTXE",
                                "id": "aabaju+0000001000451",
                                "tag": null
                            }, {
                                "parentNode": "depsecaabafa+0000001000545",
                                "type": null,
                                "childNodes": [],
                                "title": "ORTXE",
                                "id": "aabaju+0000001000453",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000545",
                        "tag": null
                    }
                ],
                "title": "Ortopedisk avdeling for OUS test",
                "id": "aabafa+0000001000545",
                "tag": null
            }, {
                "parentNode": "hosdep1",
                "type": null,
                "childNodes": [{
                        "parentNode": "aabafa+0000001000563",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depsecaabafa+0000001000563",
                                "type": null,
                                "childNodes": [],
                                "title": "Olafiaklinikken seksjon",
                                "id": "aabaju+0000001000570",
                                "tag": null
                            }
                        ],
                        "title": "Sections",
                        "id": "depsecaabafa+0000001000563",
                        "tag": null
                    }, {
                        "parentNode": "aabafa+0000001000563",
                        "type": null,
                        "childNodes": [{
                                "parentNode": "depopdaabafa+0000001000563",
                                "type": null,
                                "childNodes": [],
                                "title": "Olafiapoliklinikken",
                                "id": "aabaea+0000000000010",
                                "tag": null
                            }
                        ],
                        "title": "OPDs",
                        "id": "depopdaabafa+0000001000563",
                        "tag": null
                    }
                ],
                "title": "Olafia",
                "id": "aabafa+0000001000563",
                "tag": null
            }
        ],
        "title": "Departments",
        "id": "hosdep1",
        "tag": null
    }, {
        "parentNode": null,
        "type": null,
        "childNodes": [{
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Olafiapoliklinikken",
                "id": "aabaea+0000000000010",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Skadelegevakt",
                "id": "aabaea+0000000000011",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Røntgen sentrallab",
                "id": "aabaea+0000001000076",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Røntgen mammografi",
                "id": "aabaea+0000001000078",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Kirurgisk pol 1H",
                "id": "aabaea+0000001000096",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Kirurgisk pol 2H",
                "id": "aabaea+0000001000099",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Medisinsk pol 1H",
                "id": "aabaea+0000001000101",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Medisinsk pol 2H",
                "id": "aabaea+0000001000103",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Øye poliklinikk 1H",
                "id": "aabaea+0000001000105",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Kjevekirurgisk poliklinikk",
                "id": "aabaea+0000001000107",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Øye poliklinikk 2H",
                "id": "aabaea+0000001000109",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Øre-Nese-Hals poliklinikk",
                "id": "aabaea+0000001000111",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Rus poliklinikken",
                "id": "aabaea+0000001000113",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Psykiatrisk poliklinikk 1",
                "id": "aabaea+0000001000115",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Kirurgisk pol A",
                "id": "aabaea+0000001000117",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Kirurgisk pol B",
                "id": "aabaea+0000001000119",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Medisinsk pol A",
                "id": "aabaea+0000001000121",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Medisinsk pol B",
                "id": "aabaea+0000001000123",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Hjerte poliklinikk",
                "id": "aabaea+0000001000133",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "BHK lok1",
                "id": "aabaea+0000001000153",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "BHK lok2",
                "id": "aabaea+0000001000155",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Barn og ungdom - 1. etg",
                "id": "aabaea+0000001000186",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Barn og ungdom - 2. etg",
                "id": "aabaea+0000001000188",
                "tag": null
            }, {
                "parentNode": "hosopd1",
                "type": null,
                "childNodes": [],
                "title": "Barn og ungdom - 3. etg",
                "id": "aabaea+0000001000190",
                "tag": null
            }
        ],
        "title": "OPDs",
        "id": "hosopd1",
        "tag": null
    }
]
  