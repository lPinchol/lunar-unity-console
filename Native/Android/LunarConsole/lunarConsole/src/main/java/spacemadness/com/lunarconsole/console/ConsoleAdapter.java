//
//  ConsoleAdapter.java
//
//  Lunar Unity Mobile Console
//  https://github.com/SpaceMadness/lunar-unity-console
//
//  Copyright 2015 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

package spacemadness.com.lunarconsole.console;

import android.content.res.Resources;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import spacemadness.com.lunarconsole.R;

public class ConsoleAdapter extends RecyclerView.Adapter<ConsoleAdapter.ViewHolder>
{
    private DataSource dataSource;

    public ConsoleAdapter(DataSource dataSource)
    {
        if (dataSource == null)
        {
            throw new NullPointerException("Data source is null");
        }

        this.dataSource = dataSource;
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType)
    {
        LayoutInflater inflater = LayoutInflater.from(parent.getContext());
        View itemView = inflater.inflate(R.layout.lunar_layout_console_log_entry, parent, false);
        return new ConsoleEntry.ViewHolder(itemView);
    }

    @Override
    public void onBindViewHolder(ViewHolder holder, int position)
    {
        ConsoleEntry entry = dataSource.getEntry(position);
        holder.bindViewHolder(entry);
    }

    @Override
    public int getItemViewType(int position)
    {
        return dataSource.getEntry(position).type;
    }

    @Override
    public int getItemCount()
    {
        return dataSource.getEntryCount();
    }

    //////////////////////////////////////////////////////////////////////////////
    // Data Source

    public interface DataSource
    {
        ConsoleEntry getEntry(int position);
        int getEntryCount();
    }

    //////////////////////////////////////////////////////////////////////////////
    // View Holder

    public static abstract class ViewHolder<T extends ConsoleEntry> extends RecyclerView.ViewHolder
    {
        public ViewHolder(View itemView)
        {
            super(itemView);
        }

        void bindViewHolder(ConsoleEntry entry)
        {
            onBindViewHolder((T) entry);
        }

        public abstract void onBindViewHolder(T entry);

        protected Resources getResources()
        {
            return itemView.getContext().getResources();
        }

        protected String getString(int id)
        {
            return getResources().getString(id);
        }

        protected int getColor(int id)
        {
            return getResources().getColor(id);
        }
    }
}