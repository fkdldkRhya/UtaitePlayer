package kro.kr.rhya_network.utaiteplayer.fragment;

import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.ImageButton;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaSearchInputAdapter;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaDialogManagerNew;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHangulInitialSearch;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;

public class SearchInputFragment extends Fragment {
    private EditText searchEditText;
    private ConstraintLayout noResultLayout;
    private ImageButton xImageButton;
    // Adapter
    private RhyaSearchInputAdapter rhyaSearchInputAdapter;




    public SearchInputFragment() {
        // Required empty public constructor
    }




    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @return A new instance of fragment SearchFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static SearchInputFragment newInstance() {
        return new SearchInputFragment();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_search_input, container, false);
    }



    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);


        // Dialog 관리자
        searchEditText = view.findViewById(R.id.searchEditText);
        // UI Object
        RecyclerView recyclerViewForResult = view.findViewById(R.id.recyclerViewForResult);
        noResultLayout = view.findViewById(R.id.noResultLayout);
        xImageButton = view.findViewById(R.id.xImageButton);
        ImageButton searchImageButton = view.findViewById(R.id.searchImageButton);


        rhyaSearchInputAdapter = new RhyaSearchInputAdapter(requireActivity());


        LinearLayoutManager rhyaSearchInputAdapterLinearLayoutManager = new LinearLayoutManager(requireContext());
        recyclerViewForResult.setLayoutManager(rhyaSearchInputAdapterLinearLayoutManager);
        recyclerViewForResult.setAdapter(rhyaSearchInputAdapter);

        searchEditText.setText("");
        noResultLayout.setVisibility(View.VISIBLE);
        xImageButton.setVisibility(View.GONE);




        // Listener
        searchImageButton.setOnClickListener(v -> {
            String input = searchEditText.getText().toString();
            if (input.replaceAll(" ", "").length() >= 1)
                showSearchResult(input);

        });
        xImageButton.setOnClickListener(v -> searchEditText.setText(""));
        searchEditText.setOnKeyListener((view1, i, keyEvent) -> {
            if (i == KeyEvent.KEYCODE_ENTER) {
                String input = searchEditText.getText().toString();
                if (input.replaceAll(" ", "").length() >= 1) {
                    showSearchResult(input);
                    return true;
                }
            }

            return false;
        });
        searchEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void afterTextChanged(Editable editable) {
                if (searchEditText.getText().toString().length() >= 1) {
                    noResultLayout.setVisibility(View.GONE);
                    xImageButton.setVisibility(View.VISIBLE);

                    searchEvent();
                }else {
                    noResultLayout.setVisibility(View.VISIBLE);
                    xImageButton.setVisibility(View.GONE);

                    rhyaSearchInputAdapter.setList(new ArrayList<>(), "");
                }
            }
        });
    }


    @Override
    public void onResume() {
        super.onResume();

        searchEditText.setText(RhyaApplication.searchText);
    }

    public void searchEvent() {
        try {
            new RhyaAsyncTask<String, String>() {
                private ArrayList<String> arrayList;

                @Override
                protected void onPreExecute() {
                    arrayList = new ArrayList<>();
                }

                @Override
                protected String doInBackground(String arg) {
                    RhyaHangulInitialSearch rhyaHangulInitialSearch = new RhyaHangulInitialSearch();


                    for (String uuid : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
                        RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);
                        assert rhyaMusicInfoVO != null;

                        arg = arg.toLowerCase();

                        String songName = rhyaMusicInfoVO.getName().toLowerCase();
                        String songSinger = rhyaMusicInfoVO.getSinger().toLowerCase();


                        // 노래 비교
                        if (rhyaMusicInfoVO.getName().equalsIgnoreCase(arg) ||
                                rhyaHangulInitialSearch.matchString(songName, arg) ||
                                songName.contains(arg)) {

                            if (!arrayList.contains(rhyaMusicInfoVO.getName())) {
                                rhyaMusicInfoVO.setVersion(-1);
                                arrayList.add(rhyaMusicInfoVO.getName());
                            }
                        }else if (songSinger.equalsIgnoreCase(arg) ||
                                rhyaHangulInitialSearch.matchString(songSinger, arg) ||
                                songSinger.contains(arg)) {

                            if (!arrayList.contains(rhyaMusicInfoVO.getSinger())) {
                                rhyaMusicInfoVO.setVersion(-2);
                                arrayList.add(rhyaMusicInfoVO.getSinger());
                            }
                        }
                    }

                    return null;
                }

                @Override
                protected void onPostExecute(String result) {
                    if (arrayList.size() > 0) {
                        noResultLayout.setVisibility(View.GONE);
                    }else {
                        noResultLayout.setVisibility(View.VISIBLE);
                    }

                    rhyaSearchInputAdapter.setList(arrayList, searchEditText.getText().toString());
                }
            }.execute(searchEditText.getText().toString());
        }catch (Exception ex) {
            ex.printStackTrace();
        }
    }



    private void showSearchResult(String input) {
        ((ActivityMain) requireActivity()).showSearchResultFragment(input);
    }



    public void setSearchText(String input) {
        searchEditText.setText(input);
    }
}
