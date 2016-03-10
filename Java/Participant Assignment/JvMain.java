import java.util.*;

public class JvMain {

    public static void main(String[] args) {
        JvMain jvMain = new JvMain();
        final String[] participants = new String[] { "Kyle", "Kenny", "Eric", "Stan", "Stewie", "Brian" };
        final String[] assignments = jvMain.generateAssignments(participants);
    }

    /**
     * @param participants list of individuals participating in the draw.
     * @return the list of individuals paired with the participants where participants[0] is matched
     * with assignments[0], participants[1] is matched with assignments[1], and so on.
     */
    public String[] generateAssignments(final String[] participants) {
        Random rnd = new Random();
        List<String> assignments = new ArrayList<String>();

        // groovy version is different and better way than this
        for (int i=0; i<participants.length; i++) {
            int idx;
            do {
                idx = rnd.nextInt(participants.length);
            } while (idx == i || assignments.contains(participants[idx]));

            assignments.add(participants[idx]);
        }

        return assignments.toArray(new String[assignments.size()]);
    }

}
