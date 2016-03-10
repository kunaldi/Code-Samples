class Participation {

    /**
     * @param participants list of individuals participating in the draw.
     * @return the list of individuals paired with the participants where participants[0] is matched
     * with assignments[0], participants[1] is matched with assignments[1], and so on.
     */
    def generateAssignments(participants) {
        def unassigned = true
        def idx_list = (0..<participants.size()).toList()

        while(unassigned) {
            unassigned = false
            idx_list = idx_list.sort{ Math.random() }
            idx_list.eachWithIndex { e, i ->
                if (!unassigned) unassigned = (e == i)
            }
        }

        idx_list.inject([]) { r, p -> r << participants[p]; r }
    }
}

def main = new Participation()
def participants = ["Kyle", "Kenny", "Eric", "Stan", "Stewie", "Brian"].unique().asImmutable()
def assignments = main.generateAssignments(participants)
println(assignments.toString())