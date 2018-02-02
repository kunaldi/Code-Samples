require 'rails_helper'

RSpec.describe PostgreSQL::DatabasesController, type: :controller do

  describe 'GET index' do
    context 'existent deployment' do
      before { get :index, account_slug: 'nice-co', deployment_id: 1234 }
      it { expect(response.status).to eq(200) }
    end

    context 'inexistent deployment' do
      before { get :index, account_slug: 'nice-co', deployment_id: 12345 }
      it { expect(response.status).to eq(404) }
    end
  end

  describe 'GET live_queries' do
    context 'non xhr request ' do
      before { get :live_queries, account_slug: 'nice-co', deployment_id: 1234, database_name: 'booktown' }
      it {expect(response.status).to eq(200)}
      it {expect(response).to render_template('live_queries')}
    end

    context 'xhr request ' do
      before { xhr :get, :live_queries, account_slug: 'nice-co', deployment_id: 1234, database_name: 'booktown' }

      it 'should return status ok ' do
        expect(response.status).to eq(200)
      end

      it 'should return a timestamp in the body' do
        expect(response.body).to include('lq_timestamp')
      end
    end

  end

end
