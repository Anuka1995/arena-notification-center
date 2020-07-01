  export function findNode(array, id){
    let node;
    for (let index = 0; index < array.length; index++) {
        const element = array[index];
        if(element.id === id){
            node = element
            break
        }
        else{
            node = findNode(element.childNodes, id)
            if(node){
                break
            }
        }
    }
    return node;
}

export function findParent(array, node){
    var id = node.parentNode
    var foundNode = findNode(array, id)
    return foundNode;
}

export function getNodeNames(array, ids){

    let nodes = ids.map( id => {
        let node = findNode(array, id)
        if(node){
            if((node.title === "Sections" || node.title === "Wards" || node.title === "OPDs") && node.parentNode !== null ){
                let parent = findParent(array, node)
                return { title: `${node.title} of ${parent.title}`}
            }
            return { title: node.title, id: node.id}
        }
    })
    nodes = nodes.filter(function(element){
        if(element){
            return true
        }
        else{
            return false
        }
    })
    //console.log(nodes)
    return nodes
}

export function getOrgNamesfromUnitGids(unitGids){
    var orgTreeJson = localStorage.getItem("fullorgTree")
    if(orgTreeJson){
        const orgTree = JSON.parse(orgTreeJson)
        return getNodeNames(orgTree, unitGids)
    }
    return []
}

export function GroupExcludeReshIds(orgArray, ids){

    // get a id find its parent and get all children to find all of them is in ids

    let listToReturn = [...ids]
    //console.log(orgArray)
    //console.log(ids)
    ids.map( id => {
        let node = findNode(orgArray, id)
        let myParent = findParent(orgArray, node)
        //console.log(myParent);
        if(listToReturn.includes(myParent.id)){
            //console.log("parent on list");
            //If parent is on the list that means all the childs will be represented
            return
        }
        
        //get all children ids of the current node level
        let childIds = myParent.childNodes.map( node => {
            return node.id
        })

        // check all childs in the ids 
        // if yes, remove all from listToReturn and add its parentid 
        let all = childIds.every( e => ids.includes(e))
        //console.log("all in", all);
        if(all){
            listToReturn = listToReturn.filter(function(value){
                return !childIds.includes(value)
            })
            listToReturn.push(myParent.id)
        }
        else{
            //keep it in the listToReturn
        }
        //console.log("listToReturn", listToReturn);
    })
    return listToReturn
}