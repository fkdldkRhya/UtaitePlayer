package kro.kr.rhya_network.utaiteplayer.lib.com_github_amarjain07_StickyScrollView_1_0_2.ui.presentation;

/**
 * Created by Amar Jain on 17/03/17.
 */

public interface IStickyScrollPresentation {
    void freeHeader();
    void freeFooter();
    void stickHeader(int translationY);
    void stickFooter(int translationY);

    void initHeaderView(int id);
    void initFooterView(int id);

    int getCurrentScrollYPos();
}
