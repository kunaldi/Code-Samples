import org.junit.*

class ParticipationTest {
    def main, participants, assignments

    @Before
    void setUp() {
        main = new Participation()
        participants = ["Kyle", "Kenny", "Eric", "Stan", "Stewie", "Brian"].unique().asImmutable()
        assignments = main.generateAssignments(participants)
    }

    @Test
    void testMustBeAssignedToAnotherPerson() {
        assignments.eachWithIndex { e, i ->
            assert participants[i] != e
        }
    }

    @Test
    void testMustBeInParticipantsList() {
        assignments.each { a ->
            assert a in participants
        }
    }

    @Test
    void testAssignmentsMustBeSameAmountAsParticipants() {
        assert assignments.size() == participants.size()
    }
}
