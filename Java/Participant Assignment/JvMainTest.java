import org.junit.Before;
import org.junit.Test;

import java.util.Arrays;
import java.util.List;

public class JvMainTest {
    final String[] participants = new String[] { "Kyle", "Kenny", "Eric", "Stan", "Stewie", "Brian" };
    private String[] assignments;

    @Before
    public void setUp() throws Exception {
        JvMain jvMain = new JvMain();
        assignments = jvMain.generateAssignments(participants);
    }

    @Test
    public void testMustBeAssignedToAnotherPerson() throws Exception {
        for (int i=0; i<assignments.length; i++) {
            assert participants[i] != assignments[i];
        }
    }

    @Test
    public void testMustBeInParticipantsList() throws Exception {
        List<String> p = Arrays.asList(participants);

        for (String assignment: assignments) {
            assert p.contains(assignment);
        }
    }

    @Test
    public void testAssignmentsMustBeSameAmountAsParticipants() throws Exception {
        assert assignments.length == participants.length;
    }


}
